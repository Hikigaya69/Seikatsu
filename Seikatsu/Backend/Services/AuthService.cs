
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Exceptions;
using Seikatsu.Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                Expires = DateTime.UtcNow.AddDays(7)
                   //  sent to all refresh endpoint
            });
        }

        public void ClearTokenCookies()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,               // match your login cookie options
                SameSite = SameSiteMode.None,  // match your login cookie options
              
            };
            Response.Cookies.Delete("access_token", cookieOptions);
            Response.Cookies.Delete("refresh_token", cookieOptions);
        }

        public string? GetAccessToken() => Request.Cookies["access_token"];
        public string? GetRefreshToken() => Request.Cookies["refresh_token"];
    }



    // AuthService

    public class AuthService(
        Data.UserContext context,
        IConfiguration configuration,
        CookieService cookieService,
        IEmailService emailService,
        ICartService cartService,
        IRestockCartService restockCartService,
        IChecklistService checklistService
    ) : IAuthService
    {
        // REGISTER
        public async Task<CustomerRegisterDTO?> RegisterAsync(CustomerDTO request)
        {
            // 1. validate fields first — collect all errors together
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(request.FullName))
                errors["fullName"] = "Full name is required.";

            if (string.IsNullOrWhiteSpace(request.Email))
                errors["email"] = "Email is required.";
            else if (!request.Email.Contains("@"))
                errors["email"] = "Email is not valid.";

            if (string.IsNullOrWhiteSpace(request.Password))
                errors["password"] = "Password is required.";
            else if (request.Password.Length < 8)
                errors["password"] = "Password must be at least 8 characters.";

            // if any field failed — throw all errors at once
            if (errors.Any())
                throw new BadRequestException(errors);
            //if (await context.Customers.AnyAsync(
            //u => u.FullName.ToLower() == request.FullName.ToLower()))
            //    throw new ConflictException($"Full name '{request.FullName}' is already taken.");

            // 3. check if email already taken
            if (await context.Customers.AnyAsync(
                    u => u.Email.ToLower() == request.Email.ToLower()))
                throw new ConflictException($"Email '{request.Email}' is already registered.");

            var customer = new Customer();
            var hashedPassword = new PasswordHasher<Customer>()
                                       .HashPassword(customer, request.Password);

            customer.FullName = request.FullName.ToLower();
            customer.PasswordHashed = hashedPassword;
            customer.Email = request.Email.ToLower();

            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            await cartService.CreateCartAsync(customer.Id);
            await restockCartService.CreateRestockCartAsync(customer.Id);
            await checklistService.CreateChecklistAsync(customer.Id);

            return new CustomerRegisterDTO
            {
                FullName = customer.FullName,
                Email = customer.Email
            };
        }


        // LOGIN 
        public async Task<TokenResponseDto?> LoginAsync(CustomerLoginDTO request)
        {
            var exsistingToken = cookieService.GetAccessToken();
            if (exsistingToken != null)
            {
               var validatedToken= ValidateJwtToken(exsistingToken);
                if (validatedToken != null)
                {
                    throw new ConflictException("User is already logged in.");
                }
            }
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(request.Email))
                errors["email"] = "Email is required.";
            else if (!request.Email.Contains("@"))
                errors["email"] = "Email is not valid.";

            if (string.IsNullOrWhiteSpace(request.Password))
                errors["password"] = "Password is required.";
            else if (request.Password.Length < 8)
                errors["password"] = "Password must be at least 8 characters.";

            // if any field failed — throw all errors at once
            if (errors.Any())
                throw new BadRequestException(errors);
            var customer = await context.Customers
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
            if (customer is null)
                throw new UnauthorizedException("Invalid email or password.");


            if (new PasswordHasher<Customer>()
                    .VerifyHashedPassword(customer, customer.PasswordHashed, request.Password)
                    == PasswordVerificationResult.Failed)
                throw new UnauthorizedException("Invalid email or password.");

            var tokenResponse = await CreateTokenResponse(customer);

            // Write both tokens into HttpOnly cookies on the response
            cookieService.SetTokenCookies(tokenResponse.AccessToken, tokenResponse.RefreshToken);

            return tokenResponse;
        }


        //  LOGOUT
        public async Task<bool> LogoutAsync(Guid customerID)
        {
            var customer = await context.Customers.FindAsync(customerID)
          ?? throw new NotFoundException("Customer not found.");

            customer.RefreshToken = null;
            customer.RefreshTokenExpiryTime = null;
            await context.SaveChangesAsync();

            cookieService.ClearTokenCookies();
            return true;
        }


        // REFRESH TOKEN called when the access token is expired after 15mins
        public async Task RefreshTokenAsync()
        {
            var incomingRefreshToken = cookieService.GetRefreshToken()
                ?? throw new UnauthorizedException("No session found.");        

            var customerId = ExtractCustomerIdFromRefreshToken(incomingRefreshToken)
                ?? throw new UnauthorizedException("Invalid refresh token.");   

            var customer = await context.Customers.FindAsync(customerId)
                ?? throw new NotFoundException("Customer not found.");          

            if (customer.RefreshToken != HashToken(incomingRefreshToken))
            {
                await RevokeRefreshToken(customer);
                throw new UnauthorizedException("Token reuse detected. Please log in again."); 
            }

            if (customer.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedException("Session expired. Please log in again.");      

            var tokenResponse = await CreateTokenResponse(customer);
            cookieService.SetTokenCookies(tokenResponse.AccessToken, tokenResponse.RefreshToken);
            
        }     // FORGOT PASSWORD
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
            var resetLink = $"{clientUrl}/reset-password?email={Uri.EscapeDataString(request.Email)}&token={Uri.EscapeDataString(encodedToken)}";

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
             var errors = new Dictionary<string, string>(); 
            if (string.IsNullOrWhiteSpace(request.NewPassword))
                errors["newPassword"] = "New password is required.";
            else if (request.NewPassword.Length < 8)
                errors["newPassword"] = "New password must be at least 8 characters.";
            if (string.IsNullOrWhiteSpace(request.Email))
                errors["email"] = "Email is required.";
            else if (!request.Email.Contains("@"))
                errors["email"] = "Email is not valid.";
            // if any field failed — throw all errors at once
            if (errors.Any())
                throw new BadRequestException(errors);



            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.Email.ToLower() == request.Email.ToLower());

            if (customer is null) throw new NotFoundException("Customer not found.");

            // No reset token exists
            if (customer.PasswordResetToken is null) throw new UnauthorizedException("Unauthorized token");

            // Decode token from URL then hash it for comparison
            var decodedToken = Encoding.UTF8.GetString(
                                    WebEncoders.Base64UrlDecode(request.Token));

            // Compare hash 
            if (customer.PasswordResetToken != HashToken(decodedToken))
                throw new  UnauthorizedException("Unauthorized token");

            // Check expiry
            if (customer.PasswordResetTokenExpiry <= DateTime.UtcNow)
                throw new UnauthorizedException("Expired token");

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

        private ClaimsPrincipal? ValidateJwtToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var key = Encoding.UTF8.GetBytes(
                    configuration.GetValue<string>("AppSettings:Token")!
                );

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration.GetValue<string>("AppSettings:Issuer"),
                    ValidAudience = configuration.GetValue<string>("AppSettings:Audience"),

                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ClockSkew = TimeSpan.Zero 
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);

              

                return principal; 
            }
            catch
            {
                return null; 
            }
        }


    }

}