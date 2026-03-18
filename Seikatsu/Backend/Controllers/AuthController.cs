

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
            var result = await authService.LogoutAsync();
            if (!result)
                return Unauthorized("Logout failed.");
            return Ok(new { message = "Logged out successfully." });
        }


        // REFRESH TOKEN 
        // No request body needed for browser clients.
        // Refresh token is read from cookie inside the service.
        // UserId is extracted here from the expired access token cookie.
    
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken()
        {
            var result = await authService.RefreshTokenAsync();
            if (result is null)
                return Unauthorized("Invalid or expired refresh token.");
            return Ok(new { message = "Token refreshed." });
        }


        // AUTHENTICATED ONLY 
        //[Authorize]
        //[HttpGet]
        //public IActionResult AuthenticatedOnlyEndpoint()
        //{
        //    return Ok("You are authenticated!");
        //}


        // ADMIN ONLY 
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-endpoint")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are admin!");
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await authService.ForgotPasswordAsync(dto);

            // Always return same response — don't reveal if email exists
            return Ok(new { message = "If this email is registered, a reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await authService.ResetPasswordAsync(dto);

            if (!result)
                return BadRequest(new { message = "Invalid or expired reset token." });

            return Ok(new { message = "Password reset successful." });
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
