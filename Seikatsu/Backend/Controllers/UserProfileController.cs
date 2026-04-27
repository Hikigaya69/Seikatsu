using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Seikatsu.Backend.CommonAPIRespone;
using Seikatsu.Backend.Models;
using Seikatsu.Backend.Services;
using System.Security.Claims;

namespace Seikatsu.Backend.Controllers
{
    [EnableCors("specificOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController(IUserProfileService userProfileService) : ControllerBase 
    {
        [Authorize]
        [HttpGet("getprofile")]
        public async Task<ActionResult<APIResponse<UserProfileResponseDTO>>>GetUserProfile()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await userProfileService.GetUserDetailsAsync(customerId);
            var response = new APIResponse<UserProfileResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "User profile details are sent"
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost("updateprofile")]
        public async Task<ActionResult<APIResponse<UserProfileResponseDTO>>> GetUserProfile([FromBody] UserProfileUpdateDTO request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await userProfileService.UpdateUserInfoAsync(customerId,request);
            var response = new APIResponse<UserProfileResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "User profile sucessfully updated"
            };

            return Ok(response);
        }



        [Authorize]
        [HttpGet("getorderview")]
        public async Task<ActionResult<APIResponse<OrderOverviewDTO>>> GetOrderView()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await userProfileService.OrderStatusAsync(customerId);
            var response = new APIResponse<OrderOverviewDTO>
            {
                Success = true,
                Data = result,
                Message = "Order overview sent"
            };

            return Ok(response);
        }


    }

}
