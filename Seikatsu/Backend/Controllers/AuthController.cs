

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Seikatsu.Backend.CommonAPIRespone;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;
using Seikatsu.Backend.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Seikatsu.Backend.Controllers
{
    [EnableCors("specificOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : Controller
    {
        [Authorize]
        [HttpGet("check")]
        public IActionResult Check() {

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            return Ok(new APIResponse<object>
            {
                Success = true,
                Data = new
                {
                    Role = role,
                    Email = email
                }
            });
        }
        // REGISTER 
        [HttpPost("register")]
        public async Task<ActionResult<APIResponse<CustomerRegisterDTO>>> Register(CustomerDTO request)
        {
            var user = await authService.RegisterAsync(request);

            return Ok(new APIResponse<CustomerRegisterDTO>
            {
                Success = true,
                Data = user,       // user is already CustomerRegisterDTO — no mapping needed
                Message = "Registration successful."
            });
        }


        // LOGIN
        // Tokens are written to HttpOnly cookies inside the service.
        // Response body returns only a message — tokens are NOT exposed to JS.
        [HttpPost("login")]
        public async Task<ActionResult<APIResponse<object>>> Login(CustomerLoginDTO request)
        {
            var result = await authService.LoginAsync(request);

            return Ok(new APIResponse<object>
            {
                Success = true,
                Data = null,       
                Message = "Login successful."
            });

           
        }


        //  LOGOUT 
        // [Authorize] reads the access token from the HttpOnly cookie automatically
        // via OnMessageReceived in Program.cs
        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<APIResponse<object>>> Logout()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
             await authService.LogoutAsync(customerId);

            return Ok(new APIResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Logout successful."
            });
        }


        // REFRESH TOKEN 
        // No request body needed for browser clients.
        // Refresh token is read from cookie inside the service.
        // UserId is extracted here from the expired access token cookie.
    
        [HttpPost("refresh-token")]
        public async Task<ActionResult<APIResponse<object>>> RefreshToken()
        {
             await authService.RefreshTokenAsync();
            return Ok(new APIResponse<object>
            {
                Success = true,
                Data = null,       // user is already CustomerRegisterDTO — no mapping needed
                Message = "Token refreshed successful."
            });

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
        public async Task<ActionResult<APIResponse<object>>> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            

            await authService.ForgotPasswordAsync(dto);
           
            return Ok(new APIResponse<object>
            {
                Success = true,
                Data = null,
                Message = "If this email is registered, a reset link has been sent."
            });
            

        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<APIResponse<object>>> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            await authService.ResetPasswordAsync(dto);
            return Ok(new APIResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Password reset successful."
            });

        }

        // PRIVATE HELPER


        // Reads claims from an expired JWT without validating expiry.
        //// Used only to extract UserId for the refresh flow.
        //private static string? GetUserIdFromExpiredToken(string token)
        //{
        //    try
        //    {
        //        var handler = new JwtSecurityTokenHandler();
        //        var jwt = handler.ReadJwtToken(token);
        //        return jwt.Claims
        //                  .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
        //                  ?.Value;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
    }
}
