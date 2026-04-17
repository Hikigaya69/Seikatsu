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
    public class RestockCartController(IRestockCartService restockCartService) : ControllerBase
    {
        [Authorize]
        [HttpPost("getrestockcart")]
        public async Task<ActionResult<APIResponse<GetRestockCartDTO>>> GetCart()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await restockCartService.GetRestockCartAsync(customerId);
            var response = new APIResponse<GetRestockCartDTO>
            {
                Success = true,
                Data = result,
                Message = "restock cart with/without products is sent"
            };

            return Ok(response);


        }

        [Authorize]
        [HttpPost("additem-restockcart")]
        public async Task<ActionResult<APIResponse<AddItemtoRestockCartDTO>>> AddItemtoCart([FromBody] ItemAddtoRestockCartDTO request)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await restockCartService.AddItemToRestockCartAsync(customerId, request);
                var response = new APIResponse<AddItemtoRestockCartDTO>
                {
                    Success = true,
                    Data = result,
                    Message = result != null ? "prodcuts are sent" : "No products found."
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpDelete("items/{cartitemid:guid}")]
        public async Task<ActionResult<APIResponse<object>>> DeleteitemFromCart([FromRoute] Guid cartitemid)
        {

            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await restockCartService.DeleteItemFormRestockCartAsync(customerId, cartitemid);
            return Ok(new APIResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Item removed from cart successfully."
            });

        }


        [Authorize]
        [HttpDelete("clearrestockcart")]
        public async Task<ActionResult<APIResponse<object>>> ClearCart()
        {

            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await restockCartService.ClearRestockCartAsync(customerId);
            return Ok(new APIResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Cart cleared successfully."
            });



        }

        [Authorize]
        [HttpPatch("updaterestockcart")]

        public async Task<ActionResult<APIResponse<UpdateRestockCartResponseDTO>>> UpdateCartItem([FromBody] UpdateRestockCartRequestDTO request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await restockCartService.UpdateRestockCartItemAysnc(customerId, request);

            var response = new APIResponse<UpdateRestockCartResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "Cart item updated successfully."
            };
            return Ok(response);
        }

        [Authorize]
        [HttpPatch("updaterestockstatus")]
        public async Task<ActionResult<APIResponse<UpdateRestockCartStatusResponseDTO>>> UpdateRestockItemStatus([FromBody] RestockItemStatusRequestDTO request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
           var result= await restockCartService.SetRestockItemStatusAsync(customerId, request);
            var response = new APIResponse<UpdateRestockCartStatusResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "Cart item updated successfully."
            };
            return Ok(response);

        }
    }
}
