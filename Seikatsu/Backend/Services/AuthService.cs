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


namespace Seikatsu.Backend.Services
{
    public class AuthService(Data.UserContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<TokenResponseDto> LoginAsync(UserDTO request)
        { //login method which returns both access token and refresh token

            var user = context.Users.FirstOrDefault(u => u.Username == request.Username);
            if (user is null)
            {
                return null;
            }
            // check the passwordhash to verify the password    
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            //method to create token response
            return await CreateTokenResponse(user);

        }


        //register method to create new user    
        public async Task<User?> RegisterAsync(UserDTO request)
        {
            if (await context.Users.AnyAsync(u=> u.Username == request.Username))
            {
                return null;
            }

            var user = new User();
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);
            user.Username = request.Username;
            user.PasswordHash = hashedPassword;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;

        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RequestTokenRefreshDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);

            if (user is null)
            {
                return null;
            }

            return await CreateTokenResponse(user);
        }

        private async Task<TokenResponseDto?> CreateTokenResponse(User? user)
        {
            return new TokenResponseDto
            {

                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndStoreRefreshToken(user)
            };
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId,string refreshToken)
        {
            var user= await context.Users.FindAsync(userId);

            if(user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }


        private string GenerateRefreshToken()
        {

            var randomNumber=new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<string> GenerateAndStoreRefreshToken(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();   
            return refreshToken;    


        }

        private string CreateToken(User user)
        {
            var clamis = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.NameIdentifier,user.ID.ToString()),
                new Claim(ClaimTypes.Role,user.Roles)
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
