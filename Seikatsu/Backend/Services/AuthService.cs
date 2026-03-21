
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Seikatsu.Backend.Services
{

    // CookieService — handles all HttpOnly cookie operations

    public class CookieService(IHttpContextAccessor httpContextAccessor, IHostEnvironment env)
    {

        private HttpResponse Response => httpContextAccessor.HttpContext!.Response;
        private HttpRequest Request => httpContextAccessor.HttpContext!.Request;

        public void SetTokenCookies(string accessToken, string refreshToken)
        {


            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,                        // HTTPS only
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/api/auth/refresh-token"    // only sent to refresh endpoint
            });
        }

        public void ClearTokenCookies()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                Path = "/api/auth/refresh-token"
            });
        }

        public string? GetAccessToken() => Request.Cookies["access_token"];
        public string? GetRefreshToken() => Request.Cookies["refresh_token"];
    }



    // AuthService

    public class AuthService(
        Data.UserContext context,
        IConfiguration configuration,
        CookieService cookieService,
        IEmailService emailService
    ) : IAuthService
    {
        // REGISTER
        public async Task<CustomerRegisterDTO?> RegisterAsync(CustomerDTO request)
        {
            if (await context.Customers.AnyAsync(
                    u => u.FullName.ToLower() == request.FullName.ToLower()))
                return null;

            var customer = new Customer();
            var hashedPassword = new PasswordHasher<Customer>()
                                       .HashPassword(customer, request.Password);

            customer.FullName = request.FullName.ToLower();
            customer.PasswordHashed = hashedPassword;
            customer.Email = request.Email.ToLower();

            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            return new CustomerRegisterDTO
            {
                FullName = customer.FullName,
                Email = customer.Email
            };
        }


        // LOGIN 
        public async Task<TokenResponseDto?> LoginAsync(CustomerLoginDTO request)
        {
            var customer = context.Customers
                .FirstOrDefault(u => u.Email.ToLower() == request.Email.ToLower());

            if (customer is null)
                return null;

            if (new PasswordHasher<Customer>()
                    .VerifyHashedPassword(customer, customer.PasswordHashed, request.Password)
                    == PasswordVerificationResult.Failed)
                return null;

            var tokenResponse = await CreateTokenResponse(customer);

            // Write both tokens into HttpOnly cookies on the response
            cookieService.SetTokenCookies(tokenResponse.AccessToken, tokenResponse.RefreshToken);

            return tokenResponse;
        }


        //  LOGOUT
        public async Task<bool> LogoutAsync()
        {
            var incomingRefreshToken = cookieService.GetRefreshToken();
            if (incomingRefreshToken is null) return false;

            var customerId = ExtractCustomerIdFromRefreshToken(incomingRefreshToken);
            if (customerId is null) return false;

            var customer = await context.Customers.FindAsync(customerId);
            if (customer is null) return false;

            customer.RefreshToken = null;
            customer.RefreshTokenExpiryTime = null;
            await context.SaveChangesAsync();

            cookieService.ClearTokenCookies();
            return true;
        }


        // REFRESH TOKEN called when the access token is expired after 15mins
        public async Task<TokenResponseDto?> RefreshTokenAsync()
        {
            // Read refresh JWT from cookie — browser sends automatically
            var incomingRefreshToken = cookieService.GetRefreshToken();
            if (incomingRefreshToken is null) return null;

            // Extract CustomerId from inside the refresh JWT
            var customerId = ExtractCustomerIdFromRefreshToken(incomingRefreshToken);
            if (customerId is null) return null;

            var customer = await context.Customers.FindAsync(customerId);
            if (customer is null) return null;

            // Compare hash — never compare plain text
            if (customer.RefreshToken != HashToken(incomingRefreshToken))
            {
                await RevokeRefreshToken(customer);
                return null;
            }

            // Check expiry
            if (customer.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            // Rotate — generate new access + refresh tokens
            var tokenResponse = await CreateTokenResponse(customer);
            cookieService.SetTokenCookies(tokenResponse.AccessToken, tokenResponse.RefreshToken);

            return tokenResponse;
        }
        // FORGOT PASSWORD
        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDTO request)
        {
            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.Email.ToLower() == request.Email.ToLower());

            // Return true even if not found — prevents email enumeration
            if (customer is null) return true;

            // Generate raw token 
            var rawToken = GenerateSecureToken();

            // Store only hash in DB —
            customer.PasswordResetToken = HashToken(rawToken);
            customer.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);
            await context.SaveChangesAsync();

            // Encode for URL safety
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));

            // Read frontend URL from appsettings.json
            var clientUrl = configuration.GetValue<string>("AppSettings:ClientUrl");
            var resetLink = $"{clientUrl}/reset-password?email={request.Email}&token={encodedToken}";

            try
            {
                await emailService.SendAsync(
                    to: request.Email,
                    subject: "Reset Your Password — Seikatsu",
                    body: $@"<p>Hi {customer.FullName},</p>
                     <p>Click the link below to reset your password. This link expires in 30 minutes.</p>
                     <a href='{resetLink}'>Reset Password</a>
                     <p>If you did not request this, ignore this email.</p>"
                );
            }
            catch
            {
                return false;
            }

            return true;
        }


        // RESET PASSWORD
        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO request)
        {
            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.Email.ToLower() == request.Email.ToLower());

            if (customer is null) return false;

            // No reset token exists
            if (customer.PasswordResetToken is null) return false;

            // Decode token from URL then hash it for comparison
            var decodedToken = Encoding.UTF8.GetString(
                                    WebEncoders.Base64UrlDecode(request.Token));

            // Compare hash 
            if (customer.PasswordResetToken != HashToken(decodedToken))
                return false;

            // Check expiry
            if (customer.PasswordResetTokenExpiry <= DateTime.UtcNow)
                return false;

            // Hash new password — same PasswordHasher  used in RegisterAsync
            customer.PasswordHashed = new PasswordHasher<Customer>()
                                            .HashPassword(customer, request.NewPassword);

            // Clear token — can't be reused
            customer.PasswordResetToken = null;
            customer.PasswordResetTokenExpiry = null;

            await context.SaveChangesAsync();
            return true;
        }




        // PRIVATE HELPERS

        // Generates a cryptographically secure random token
        // Same logic as your commented out GenerateRefreshToken()
        private static string GenerateSecureToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
        private async Task<TokenResponseDto> CreateTokenResponse(Customer customer)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(customer),
                RefreshToken = await GenerateAndStoreRefreshToken(customer)
            };
        }

        // Wipes refresh token in DB and clears cookies — called on reuse detection
        private async Task RevokeRefreshToken(Customer customer)
        {
            customer.RefreshToken = null;
            customer.RefreshTokenExpiryTime = null;
            await context.SaveChangesAsync();
            cookieService.ClearTokenCookies();
        }

        //private static string GenerateRefreshToken()
        //{
        //    var randomBytes = new byte[64];
        //    using var rng = RandomNumberGenerator.Create();
        //    rng.GetBytes(randomBytes);
        //    return Convert.ToBase64String(randomBytes);
        //}

        private async Task<string> GenerateAndStoreRefreshToken(Customer customer)
        {
            var refreshToken = GenerateRefreshJwt(customer); // signed JWT with CustomerId inside
            customer.RefreshToken = HashToken(refreshToken); // store only the hash in DB
            customer.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken; // raw JWT goes into cookie
        }







        private Guid? ExtractCustomerIdFromRefreshToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        configuration.GetValue<string>("AppSettings:Token")!));

                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true // refresh token expiry enforced here
                    }, out _);

                var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return idClaim is null ? null : Guid.Parse(idClaim);
            }
            catch
            {
                return null; // tampered or expired
            }
        }


        private string GenerateRefreshJwt(Customer customer)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString())
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    configuration.GetValue<string>("AppSettings:Token")!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddDays(7),
                claims: claims,
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        private string CreateToken(Customer customer)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,           customer.FullName.ToLower()),
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Email,          customer.Email.ToLower()),
                // new Claim(ClaimTypes.Role,        customer.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    configuration.GetValue<string>("AppSettings:Token")!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),  // short-lived
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}