using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Seikatsu.Backend.CommonAPIRespone;
using Seikatsu.Backend.Services;
using Microsoft.AspNetCore.Cors;
namespace Seikatsu.Backend.Controllers
{

    [EnableCors("specificOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController (IProductService productService) : ControllerBase
    {
        [HttpGet("productforindex")]
        public async Task<ActionResult<IEnumerable<ProductDTOforIndexPage>>> GetRandomProducts(int count = 10)
        {
            var products = await productService.GetRandomProductsAsync(count);
            var response = new APIResponse<IEnumerable<ProductDTOforIndexPage>>
            {
                Success = true,
                Data = products,
                Message = products.Any() ? null : "No products found."
            };
            
            return Ok(response);
        }
    }
}
