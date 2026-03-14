

using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Seikatsu.Backend.Services;

namespace Seikatsu.Backend.Controllers
{
    [EnableCors("specificOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : Controller
    {
        // REGISTER 
        [HttpPost("register")]
        public async Task<ActionResult<CustomerRegisterDTO>> Register(CustomerDTO request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
                return BadRequest("User already exists.");

            return Ok(user);
        }


        // LOGIN
        // Tokens are written to HttpOnly cookies inside the service.
        // Response body returns only a message — tokens are NOT exposed to JS.
        [HttpPost("login")]
        public async Task<ActionResult> Login(CustomerLoginDTO request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
                return BadRequest("Invalid username or password.");

            return Ok(new { message = "Login successful." });
        }


        //  LOGOUT 
        // [Authorize] reads the access token from the HttpOnly cookie automatically
        // via OnMessageReceived in Program.cs
        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return BadRequest("Logout failed.");

            var result = await authService.LogoutAsync(Guid.Parse(userId));
            if (!result)
                return BadRequest("Logout failed.");

            return Ok(new { message = "Logged out successfully." });
        }


        // REFRESH TOKEN 
        // No request body needed for browser clients.
        // Refresh token is read from cookie inside the service.
        // UserId is extracted here from the expired access token cookie.
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken()
        {
            // Read userId from the expired access token cookie
            var accessToken = Request.Cookies["access_token"];
            if (accessToken is null)
                return Unauthorized("No access token found.");

            var userId = GetUserIdFromExpiredToken(accessToken);
            if (userId is null)
                return Unauthorized("Invalid access token.");

            // RefreshToken is read from cookie inside RefreshTokenAsync
            var request = new RequestTokenRefreshDto { UserId = Guid.Parse(userId) };
            var result = await authService.RefreshTokenAsync(request);

            if (result is null)
                return Unauthorized("Invalid or expired refresh token.");

            // New cookies are already set inside the service
            return Ok(new { message = "Token refreshed." });
        }


        // ── AUTHENTICATED ONLY 
        //[Authorize]
        //[HttpGet]
        //public IActionResult AuthenticatedOnlyEndpoint()
        //{
        //    return Ok("You are authenticated!");
        //}


        // ── ADMIN ONLY 
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-endpoint")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are admin!");
        }


        // PRIVATE HELPER
       

        // Reads claims from an expired JWT without validating expiry.
        // Used only to extract UserId for the refresh flow.
        private static string? GetUserIdFromExpiredToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                return jwt.Claims
                          .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                          ?.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
