
using Microsoft.AspNetCore.Identity;
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
        CookieService cookieService
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


        // REFRESH TOKEN 
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


        // PRIVATE HELPERS


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