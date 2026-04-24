using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Seikatsu.Backend.CommonAPIRespone;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;
using Seikatsu.Backend.Models.AdminDTOs;
using Seikatsu.Backend.Services;
using System.Security.Claims;


namespace Seikatsu.Backend.Controllers

{
    [EnableCors("specificOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class AdminController(IAdminService adminService, IConfiguration configuration) : ControllerBase
    {
        [HttpPost("adminaddproducts")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<APIResponse<ProductAddResponseDTO>>> AddProduct(ProductAddRequestDTO request)
        {


            var result = await adminService.AddProductAsync(request);
            var response = new APIResponse<ProductAddResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "Product addedd"
            };
            return Ok(response);


        }
        [HttpGet("admingetproductview/{id}")]

        public async Task<ActionResult<APIResponse<ProductDTO>>> GetProductbyID([FromRoute] Guid id)
        {


            var product = await adminService.GetProductByIdAsync(id);
            var response = new APIResponse<ProductDTO>
            {
                Success = true,
                Data = product,
                Message = "prodcut detail is sent"
            };

            return Ok(response);

        }

        [HttpPatch("editproduct/{id}")]
        public async Task<ActionResult<APIResponse<ProductDTO>>> EditProduct([FromRoute] Guid id, EditProductRequestDTO request)
        {
            var result = await adminService.EditProductAsync(id, request);
            var response = new APIResponse<ProductDTO>
            {
                Success = true,
                Data = result,
                Message = "product is edited"
            };
            return Ok(response);
        }

        [HttpDelete("deleteproduct/{id}")]
        public async Task<ActionResult<APIResponse<object>>> DeleteProduct([FromRoute] Guid id)
        {
            var result = await adminService.DeleteProductAsync(id);
            var response = new APIResponse<object>
            {
                Success = result,
                Data = null,
                Message = result ? "product is deleted" : "product not found"
            };
            return Ok(response);

        }
        [HttpPost("createcategory")]
        public async Task<ActionResult<APIResponse<CreateCategoryResponseDTO>>> CreateCategory([FromBody] string categoryName)
        {
            var result = await adminService.CreateCategoryAsync(categoryName);
            var response = new APIResponse<CreateCategoryResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "category is created"
            };
            return Ok(response);
        }
        [HttpDelete("deletecategory/{id}")]
        public async Task<ActionResult<APIResponse<object>>> DeleteCategory([FromRoute] Guid id)
        {
            var result = await adminService.DeleteCategoryAsync(id);
            var response = new APIResponse<object>
            {
                Success = result,
                Data = null,
                Message = result ? "category is deleted" : "category not found"
            };
            return Ok(response);
        }
        [HttpGet("customerscount")]
        public async Task<ActionResult<APIResponse<int>>> CountCustomers()
        {
            var result = await adminService.CountCustomersAsync();
            var response = new APIResponse<int>
            {
                Success = true,
                Data = result,
                Message = "total customers count is sent"
            };
            return Ok(response);
        }

        [HttpGet("totalrevenue")]
        public async Task<ActionResult<APIResponse<decimal>>> TotalRevenue()
        {
            var result = await adminService.TotalRevenueEarnedAsync();
            var response = new APIResponse<decimal>
            {
                Success = true,
                Data = result,
                Message = "total revenue is sent"
            };
            return Ok(response);
        }
        [HttpGet("revenuedaterange")]
        public async Task<ActionResult<APIResponse<decimal>>> RevenueinRange([FromBody] DateRequestDTO request)
        {
            var result = await adminService.GetRevenueAsync(request);
            var response = new APIResponse<decimal>
            {
                Success = true,
                Data = result,
                Message = "revenue in the date range is sent"
            };
            return Ok(response);
        }

        [HttpGet("totalorders")]
        public async Task<ActionResult<APIResponse<OrderCountResponseDTO>>> TotalOrders()
        {
            var result = await adminService.TotalOrdersAsync();
            var response = new APIResponse<OrderCountResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "total orders count is sent"
            };
            return Ok(response);
        }
        [HttpGet("ordersinrange")]
        public async Task<ActionResult<APIResponse<OrderCountResponseDTO>>> OrdersinRange([FromBody] DateRequestDTO request)
        {
            var result = await adminService.TotalOrdersinRangeAsync(request);
            var response = new APIResponse<OrderCountResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "orders count in the date range is sent"
            };
            return Ok(response);
        }

        [HttpGet("getallcats")]
        public async Task<ActionResult<APIResponse<IEnumerable<CategoryResponseDTO>>>> GetAllCategories()
        {
            var result = await adminService.GetAllCategoriesAsync();
            var response = new APIResponse<IEnumerable<CategoryResponseDTO>>
            {
                Success = true,
                Data = result,
                Message = "all categories are sent"
            };
            return Ok(response);
        }

        [HttpGet("getproductsbycat/{id}")]
        public async Task<ActionResult<APIResponse<IEnumerable<ProductDTOforIndexPage>>>> GetProductsByCategory([FromRoute] Guid id)
        {
            var result = await adminService.GetProductByCategory(id);
            var response = new APIResponse<IEnumerable<ProductDTOforIndexPage>>
            {
                Success = true,
                Data = result,
                Message = "products in the category are sent"
            };
            return Ok(response);
        }

        [HttpGet("getproductsbycountry")]
        public async Task<ActionResult<APIResponse<IEnumerable<ProductCountryDTO>>>> GetProductsByCountry([FromQuery] string countryname)
        {
            var result = await adminService.GetProductbyCountryAsync(countryname);
            var response = new APIResponse<IEnumerable<ProductCountryDTO>>
            {
                Success = true,
                Data = result,
                Message = "products in the country are sent"
            };
            return Ok(response);
        }

        //this for plotting graphs.. If categoryId can be null also.. if choosen the analysis will be for that perticular category
        [HttpGet("revenueforperiod")]
        public async Task<ActionResult<APIResponse<RevenueResponseDTO>>> RevenueForPeriod([FromBody] RevenuePeriodRequestDTO request)
        {
            var result = await adminService.GetRevenueForPeriodAsync(request);
            var response = new APIResponse<RevenueResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "revenue for the period is sent"
            };
            return Ok(response);
        }
        //this for plotting graphs... here ddont mind the request dto name.. fileds are same
        [HttpGet("ordersforperiod")]
        public async Task<ActionResult<APIResponse<OrderAnalysisResponseDTO>>> OrdersForPeriod([FromBody] RevenuePeriodRequestDTO request)
        {
            var result = await adminService.GetOrderForPeriodAsync(request);
            var response = new APIResponse<OrderAnalysisResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "orders for the period is sent"
            };
            return Ok(response);
        }

        [HttpGet("categorywiseproductssold/{id}")]
        public async Task<ActionResult<APIResponse<int>>> CategorywiseProductsSold([FromRoute] Guid id)
        {
            var result = await adminService.CategorywiseProductsSoldAsync(id);
            var response = new APIResponse<int>
            {
                Success = true,
                Data = result,
                Message = "total products sold in the category is sent"
            };
            return Ok(response);
        }

        [HttpGet("countrywiseproductssold")]
        public async Task<ActionResult<APIResponse<int>>> CountrywiseProductsSold([FromQuery] string countryName)
        {
            var result = await adminService.CountrywiseProductsSoldAsync(countryName);
            var response = new APIResponse<int>
            {
                Success = true,
                Data = result,
                Message = "total products sold in the country is sent"
            };
            return Ok(response);
        }
        [HttpGet("categoryitemcount")]
        public async Task<ActionResult<APIResponse<IEnumerable<CategoryDetailResponseDTO>>>> CategoryItemCount()
        {
            var result = await adminService.ShowCategoryItemCount();
            var response = new APIResponse<IEnumerable<CategoryDetailResponseDTO>>
            {
                Success = true,
                Data = result,
                Message = "item count for each category is sent"
            };
            return Ok(response);
        }
        [HttpGet("countryitemcount")]
        public async Task<ActionResult<APIResponse<IEnumerable<CountryCountResponseDTO>>>> CountryItemCount()
        {
            var result = await adminService.ShowCountryItemCount();
            var response = new APIResponse<IEnumerable<CountryCountResponseDTO>>
            {
                Success = true,
                Data = result,
                Message = "item count for each country is sent"
            };
            return Ok(response);
        }
    }

}
