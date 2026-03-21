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
    public class ProductController(IProductService productService) : ControllerBase
    {
        [HttpGet("productforindex")]
        // This endpoint retrieves a list of random products for the index page. The client can specify the number of products
        // to retrieve using the 'count' query parameter, which defaults to 10 if not provided. The response is wrapped in an
        // APIResponse object that indicates success and includes the data or an appropriate message if no products are found.
        //https://localhost:7115/api/Product/GetRandomProducts this is the endpoint which the frontend will call to get the random products for the index page.     

        public async Task<ActionResult<APIResponse<IEnumerable<ProductDTOforIndexPage>>>> GetRandomProducts(int count = 10)
        {
            var products = await productService.GetRandomProductsAsync(count);
            var response = new APIResponse<IEnumerable<ProductDTOforIndexPage>>
            {
                Success = true,
                Data = products,
                Message = products.Any() ? "prodcuts are sent" : "No products found."
            };

            return Ok(response);
        }

        [HttpGet("productview/{id}")]
        public async Task<ActionResult<APIResponse<IEnumerable<ProductDTO>>>> GetProductbyID(Guid id)
        {


            var product = await productService.GetPrductbyIdAsync(id);
            var response = new APIResponse<IEnumerable<ProductDTO>>
            {
                Success = true,
                Data = product,
                Message = product.Any() ? "prodcut detail is sent" : "No products found."
            };

            return Ok(response);

        }

        [HttpGet("productbycountry/{countryname}")]

        public async Task<ActionResult<APIResponse<IEnumerable<ProductCountryDTO>>>>GetProductbyCountry(string countryname)
        {
            var products = await productService.GetProductbyCountryAsync(countryname);
            var response = new APIResponse<IEnumerable<ProductCountryDTO>>
            {
                Success = true,
                Data = products,
                Message = products.Any() ? "prodcuts for requested country are sent" : "No products found."
            };
            return Ok(response);

        }
        [HttpGet("suggestion/{query}")]
        public async Task<ActionResult<APIResponse<IEnumerable<ProductSuggestionDTO>>>> GetProductSuggestion(string query)
        {
            var suggestions = await productService.GetProductSuggestionAsync(query);
            var response = new APIResponse<IEnumerable<ProductSuggestionDTO>>
            {
                Success = true,
                Data = suggestions,
                Message = suggestions.Any() ? "product suggestions are sent" : "No products found."
            };
            return Ok(response);


        }
        [HttpGet("search/{query}")]
        public async Task<ActionResult<APIResponse<IEnumerable<ProductDTOforIndexPage>>>> GetProductsbySearch(string query)
        {
            var products = await productService.GetProductsbySearchAsync(query);
            var response = new APIResponse<IEnumerable<ProductDTOforIndexPage>>
            {
                Success = true,
                Data = products,
                Message = products.Any() ? "products matching search query are sent" : "No products found."
            };
            return Ok(response);
        }

        [HttpGet("category/{categoryId}")]

        public async Task<ActionResult<APIResponse<IEnumerable<ProductDTOforIndexPage>>>> GetProductbyCategory([FromRoute] Guid categoryId)
        {
            var products = await productService.GetProductbyCategoty(categoryId);
            var response = new APIResponse<IEnumerable<ProductDTOforIndexPage>>
            {
                Success = true,
                Data = products,
                Message = products.Any() ? "products for requested category are sent" : "No products found."
            };
            return Ok(response);
        }
    }
}