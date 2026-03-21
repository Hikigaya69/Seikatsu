using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Seikatsu.Backend.CommonAPIRespone;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;
using Seikatsu.Backend.Services;
using System.Security.Claims;

namespace Seikatsu.Backend.Controllers
{
    [EnableCors("specificOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController(ICartService cartservice, IConfiguration
        configuration) : ControllerBase
    {
        [Authorize]
        [HttpPost("getcart")]
        public async Task<ActionResult<APIResponse<IEnumerable<GetCartDTO>>>> GetCart()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
                var result = await cartservice.GetCartAysnc(customerId);
                var response = new APIResponse<IEnumerable<GetCartDTO>>
                {
                    Success = true,
                    Data = result,
                    Message = result.Any() ? "prodcuts are sent" : "No products found."
                };

                return Ok(response);
            
           
        }

        [Authorize]
        [HttpPost("additem")]
        public async Task<ActionResult<APIResponse<AddItemtoCartDTO>>> AddItemtoCart([FromBody] ItemAddFieldDTO request)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await cartservice.AddItemtoCartAysnc(customerId, request);
                var response = new APIResponse<AddItemtoCartDTO>
                {
                    Success = true,
                    Data = result,
                    Message = result!=null ? "prodcuts are sent" : "No products found."
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
                 await cartservice.DeleteItemFormCartAsync(customerId, cartitemid);
               return Ok(new APIResponse<object>
                {
                    Success = true,
                    Data = null,
                    Message = "Item removed from cart successfully."
                });

        }
           

        [Authorize]
        [HttpDelete("clearcart")]
        public async Task<ActionResult<APIResponse<object>>> ClearCart()
        {
            
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                 await cartservice.ClearCartAsync(customerId);
                return Ok(new APIResponse<object> { 
                    Success = true,
                    Data = null,
                    Message = "Cart cleared successfully."
                });



        }

        [Authorize]
        [HttpPatch("updatecart")]

        public async Task<ActionResult<APIResponse<UpdateCartResponseDTO>>> UpdateCartItem([FromBody] UpdateCartDTO request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await cartservice.UpdateCartItemAysnc(customerId, request);
                
                var response = new APIResponse<UpdateCartResponseDTO>
                {
                    Success = true,
                    Data = result,
                    Message = "Cart item updated successfully."
                };
                return Ok(response);
            }
            

        [Authorize]
        [HttpGet("cartsummary")]
        public async Task<ActionResult<APIResponse<CartSummaryDTO>>> GetCartSummary()
        {
           
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await cartservice.GetCartSummaryAsync(customerId);
               
                var response = new APIResponse<CartSummaryDTO>
                {
                    Success = true,
                    Data = result,
                    Message = "Cart summary retrieved successfully."
                };
                return Ok(response);
            }
          
    }
}