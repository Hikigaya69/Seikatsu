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
    }
}
