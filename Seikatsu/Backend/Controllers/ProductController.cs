
using Seikatsu.Backend.Models;



using Microsoft.AspNetCore.Mvc;

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


        public async Task<ActionResult<APIResponse<PagedResult<ProductDTOforIndexPage>>>> GetRandomProducts(
    [FromQuery] int pageSize = 20,
    [FromQuery] DateTime? cursorDate = null,
    [FromQuery] Guid? cursorId = null)
        {
            var result = await productService.GetRandomProductsAsync(pageSize, cursorDate);
            var response = new APIResponse<PagedResult<ProductDTOforIndexPage>>
            {
                Success = true,
                Data = result,
                Message = result.Items.Any() ? "Products are sent." : "No products found."
            };
            return Ok(response);
        }

        [HttpGet("productview/{id}")]
        public async Task<ActionResult<APIResponse<ProductDTO>>> GetProductbyID(Guid id)
        {


            var product = await productService.GetPrductbyIdAsync(id);
            var response = new APIResponse<ProductDTO>
            {
                Success = true,
                Data = product,
                Message = "prodcut detail is sent"
            };

            return Ok(response);

        }

        [HttpGet("productbycountry/{countryname}")]

        public async Task<ActionResult<APIResponse<IEnumerable<ProductCountryDTO>>>> GetProductbyCountry(string countryname)
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
        public async Task<ActionResult<APIResponse<PagedResult<ProductDTOforIndexPage>>>> GetProductByCategory(
    Guid categoryId,
    [FromQuery] int pageSize = 20,
    [FromQuery] DateTime? cursorDate = null)
        {
            var result = await productService.GetProductByCategoryAsync(categoryId, pageSize, cursorDate);
            var response = new APIResponse<PagedResult<ProductDTOforIndexPage>>
            {
                Success = true,
                Data = result,
                Message = result.Items.Any() ? "Products are sent." : "No products found."
            };
            return Ok(response);
        }
    

    [HttpGet("filter")]
        public async Task<ActionResult<APIResponse<PagedResult<ProductDTOforIndexPage>>>> GetProductByFilter(
        [FromQuery] ProductFilterRequestDTO request,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? cursorDate = null)
        {
            var result = await productService.GetProductByFilterAsync(request, pageSize, cursorDate);
            var response = new APIResponse<PagedResult<ProductDTOforIndexPage>>
            {
                Success = true,
                Data = result,
                Message = result.Items.Any() ? "Products are sent." : "No products found."
            };
            return Ok(response);
        }

        [HttpGet("allcountries")]
        public async Task<ActionResult<APIResponse<IEnumerable<CountryDTO>>>> GetAllCountries()
        {
            var countries = await productService.GetAllCountryAsync();
            var response = new APIResponse<IEnumerable<CountryDTO>>
            {
                Success = true,
                Data = countries,
                Message = countries.Any() ? "Countries are sent." : "No countries found."
            };
            return Ok(response);
        }
    }
}