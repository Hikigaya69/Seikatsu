using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Seikatsu.Backend.CommonAPIRespone;
using Seikatsu.Backend.Models;
using Seikatsu.Backend.Services;

namespace Seikatsu.Backend.Controllers
{
    [EnableCors("specificOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        [HttpGet("getallcategories")]
        public async Task<ActionResult<IEnumerable<Models.CategoryResponseDTO>>> GetAllCategories()
        {
            var categories = await categoryService.GetAllCategoriesAsync();
            var response = new APIResponse<IEnumerable<CategoryResponseDTO>>
            {
                Success = true,
                Data = categories,
                Message = categories.Any() ? "categories are sent" : "No products found."
            };

            return Ok(response);
        }
    }
}
