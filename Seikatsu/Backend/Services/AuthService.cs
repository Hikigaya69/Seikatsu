using Seikatsu.Backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Identity.Client;


namespace Seikatsu.Backend.Services
{
    public class AuthService(Data.UserContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<TokenResponseDto> LoginAsync(CustomerDTO request)
        { //login method which returns both access token and refresh token

            var customer = context.Customers.FirstOrDefault(u => u.FullName.ToLower() == request.FullName.ToLower() || u.Email.ToLower()    
            == request.Email.ToLower());
            if (customer is null)
            {
                return null;
            }
            // check the passwordhash to verify the password    
            if (new PasswordHasher<Customer>().VerifyHashedPassword(customer, customer.PasswordHashed, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            //method to create token response
            return await CreateTokenResponse(customer);

        }

        public async Task<bool>LogoutAsync(Guid userId)
        {
            var customer= await context.Customers.FindAsync(userId);

            if(customer is null)
            {
                return false;
            }
            customer.RefreshToken = null;
            customer.RefreshTokenExpiryTime = null;
            await context.SaveChangesAsync();
            return true;
        }


        //register method to create new user    
        public async Task<Customer?> RegisterAsync(CustomerDTO request)
        {
            if (await context.Customers.AnyAsync(u=> u.FullName.ToLower() == request.FullName.ToLower()))
            {
                return null;
            }

            var customer = new Customer();
            var hashedPassword = new PasswordHasher<Customer>().HashPassword(customer, request.Password);
            customer.FullName = request.FullName.ToLower();
            customer.PasswordHashed = hashedPassword;
            customer.Email = request.Email.ToLower();
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            return customer;

        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RequestTokenRefreshDto request)
        {
            var customer = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);

            if (customer is null)
            {
                return null;
            }

            return await CreateTokenResponse(customer);
        }

        private async Task<TokenResponseDto?> CreateTokenResponse(Customer? customer)
        {
            return new TokenResponseDto
            {

                AccessToken = CreateToken(customer),
                RefreshToken = await GenerateAndStoreRefreshToken(customer)
            };
        }

        private async Task<Customer?> ValidateRefreshTokenAsync(Guid userId,string refreshToken)
        {
            var customer= await context.Customers.FindAsync(userId);

            if(customer is null || customer.RefreshToken != refreshToken || customer.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return customer;
        }


        private string GenerateRefreshToken()
        {

            var randomNumber=new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<string> GenerateAndStoreRefreshToken(Customer customer)
        {
            var refreshToken = GenerateRefreshToken();
            customer.RefreshToken = refreshToken;
            customer.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();   
            return refreshToken;    


        }

        private string CreateToken(Customer customer)
        {
            var clamis = new List<Claim>
            {
                new Claim(ClaimTypes.Name,customer.FullName.ToLower()),
                new Claim(ClaimTypes.NameIdentifier,customer.Id.ToString()),
                new Claim(ClaimTypes.Email,customer.Email.ToLower())
              //  new Claim(ClaimTypes.Role,user.Roles)
            };

            var key = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken
                (issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: clamis,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: cred
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

       
    }
}
