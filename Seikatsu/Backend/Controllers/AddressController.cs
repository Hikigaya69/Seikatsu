using Microsoft.AspNetCore.Mvc;
using Seikatsu.Backend.Services;
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
    public class AddressController(IAddressService addressService, IConfiguration configuration) : ControllerBase
    {


        [Authorize]
        [HttpGet("getalladdress")]
        public async Task<ActionResult<IEnumerable<AddressResponseDTO>>> GetAllAddress()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var result = await addressService.GetAllAddressAysnc(customerId);
                var response = new APIResponse<IEnumerable<AddressResponseDTO>>
                {
                    Success = true,
                    Data = result,
                    Message = result.Any() ? "all address are sent" : "No address found."
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("getDefaultAddress")]

        public async Task<ActionResult<IEnumerable<AddressResponseDTO>>> GetDefaultAddress()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var result = await addressService.GetDefaultAddressAysnc(customerId);
                var response = new APIResponse<IEnumerable<AddressResponseDTO>>
                {
                    Success = true,
                    Data = result,
                    Message = result.Any() ? "Default address is sent" : "No defaultadrress found."
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }


        }

        [Authorize]
        [HttpPost("addAddress")]

        public async Task<ActionResult<AddressResponseDTO>> AddAddress([FromBody] AddAddressDTO request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var result = await addressService.AddAddressAsync(customerId, request);
                var response = new APIResponse<AddressResponseDTO>
                {
                    Success = true,
                    Data = result,
                    Message = "Address added successfully."
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("deleteAddress{id}")]
        public async Task<ActionResult> DeleteAddress([FromRoute] Guid id)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var result = await addressService.DeleteAddressAsync(customerId, id);
                if (!result)
                    return NotFound(new { message = "Address not found." });
                return Ok(new { message = "Address deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [Authorize]
        [HttpPut("updateAddress{addressId}")]

        public async Task<ActionResult<UpdateAddressDTO>> UpdateAddress([FromRoute] Guid addressId, [FromBody] UpdateAddressDTO request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var result = await addressService.UpdateAddressAsync(customerId, request, addressId);
                if (result is null)
                    return NotFound(new { message = "Address not found." });
                var response = new APIResponse<UpdateAddressDTO>
                {
                    Success = true,
                    Data = result,
                    Message = "Address updated successfully."
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