using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Seikatsu.Backend.Services;

namespace Seikatsu.Backend.Controllers

{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : Controller
    {

        [HttpPost("register")]
        //actionreslut can be used in controller actions to return different types of responses such as success, bad request, not found etc.
        //and should be used in controlles only.
        // here UserDTO is used to get the username and password from the client and as the success response it returns the User entity
        public async Task<ActionResult<User>> Register(UserDTO request)
        {

            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return BadRequest("User already exists.");
            }
            return Ok(user); //returns the created user entity   to client

        }

        [HttpPost("login")]

        public async Task<ActionResult<TokenResponseDto>> Login(UserDTO request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                return BadRequest("Invalid username or password.");
            }
            return Ok(result);

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
