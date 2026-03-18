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
        public async Task<ActionResult<IEnumerable<GetCartDTO>>> GetCart()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var result = await cartservice.GetCartAysnc(customerId);
                var response = new APIResponse<IEnumerable<GetCartDTO>>
                {
                    Success = true,
                    Data = result,
                    Message = result.Any() ? "prodcuts are sent" : "No products found."
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("additem")]
        public async Task<ActionResult<AddItemtoCartDTO>> AddItemtoCart([FromBody] ItemAddFieldDTO request)
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
        public async Task<ActionResult> DeleteitemFromCart([FromRoute] Guid cartitemid)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await cartservice.DeleteItemFormCartAsync(customerId, cartitemid);
                if (!result)
                    return NotFound(new { message = "Cart or item not found." });
                return Ok(new { message = "Item removed from cart successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("clearcart")]
        public async Task<ActionResult> ClearCart()
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await cartservice.ClearCartAsync(customerId);
                if (!result)
                    return NotFound(new { message = "Cart not found." });
                return Ok(new { message = "Cart cleared successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPatch("updatecart")]

        public async Task<ActionResult<UpdateCartResponseDTO>> UpdateCartItem([FromBody] UpdateCartDTO request)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await cartservice.UpdateCartItemAysnc(customerId, request);
                if (result == null)
                    return NotFound(new { message = "Cart or item not found." });
                var response = new APIResponse<UpdateCartResponseDTO>
                {
                    Success = true,
                    Data = result,
                    Message = "Cart item updated successfully."
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [Authorize]
        [HttpGet("cartsummary")]
        public async Task<ActionResult<CartSummaryDTO>> GetCartSummary()
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await cartservice.GetCartSummaryAsync(customerId);
                if (result == null)
                    return NotFound(new { message = "Cart not found." });
                var response = new APIResponse<CartSummaryDTO>
                {
                    Success = true,
                    Data = result,
                    Message = "Cart summary retrieved successfully."
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}