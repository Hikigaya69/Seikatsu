using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.IdentityModel.Tokens.Jwt;

using System.Text;

using Seikatsu.Backend.Services;
using Microsoft.AspNetCore.Cors;
using System.Security.Claims;

namespace Seikatsu.Backend.Controllers

{
    [EnableCors("specificOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : Controller
    {

        [HttpPost("register")]
        //actionreslut can be used in controller actions to return different types of responses such as success, bad request, not found etc.
        //and should be used in controlles only.
        // here UserDTO is used to get the username and password from the client and as the success response it returns the User entity
        public async Task<ActionResult<CustomerDTO>> Register(CustomerDTO request)
        {

            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return BadRequest("User already exists.");
            }
            return Ok(user); //returns the created user entity   to client

        }

        [HttpPost("login")]

        public async Task<ActionResult<TokenResponseDto>> Login(CustomerDTO request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                return BadRequest("Invalid username or password.");
            }
            return Ok(result);

        }
        [Authorize]
        [HttpPost("logout")]

        public async Task<ActionResult> Logout()
        {
            var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId==null)
            {
                return BadRequest("Logout failed.");
            }
            var result = await authService.LogoutAsync(Guid.Parse(userId));
            if (!result)
            {
                return BadRequest("Logout failed.");
            }
            return Ok("Logged out successfully.");
        }



        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authenticated!");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("admin-endpoint")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are admin!");
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RequestTokenRefreshDto request)
        {
            var result = await authService.RefreshTokenAsync(request);
            if (result is null)
            {
                return BadRequest("Invalid token or user ID.");
            }
            return Ok(result);



        }
    }
}
