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
        // This endpoint retrieves a list of random products for the index page. The client can specify the number of products
        // to retrieve using the 'count' query parameter, which defaults to 10 if not provided. The response is wrapped in an
        // APIResponse object that indicates success and includes the data or an appropriate message if no products are found.
        //https://localhost:7115/api/Product/GetRandomProducts this is the endpoint which the frontend will call to get the random products for the index page.     

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
