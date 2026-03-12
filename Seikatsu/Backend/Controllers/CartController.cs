using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Seikatsu.Backend.CommonAPIRespone;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;
using Seikatsu.Backend.Services;

namespace Seikatsu.Backend.Controllers
{
    [EnableCors("specificOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController(ICartService cartservice) : ControllerBase
    {
        [Authorize]
        [HttpPost("additemtocart")]
        public async Task<ActionResult<IEnumerable<AddItemtoCartDTO>>> AddItemstoCart(Guid customerid, Guid productid, int quantity)
        {
            try
            {
                var result = await cartservice.AddItemstoCartAysnc(customerid, productid, quantity);
                var response = new APIResponse<IEnumerable<AddItemtoCartDTO>>
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
    }
}
