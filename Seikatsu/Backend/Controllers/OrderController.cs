using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Seikatsu.Backend.CommonAPIRespone;
using Seikatsu.Backend.Models;
using Seikatsu.Backend.Services;
using System.Security.Claims;

namespace Seikatsu.Backend.Controllers
{
    [EnableCors("specificOrigins")]
    [ApiController]
    [Route("api/[controller]")]

    public class OrderController(IOrderService orderService) : ControllerBase
    {
        //https://localhost:7115/api/order/initiateorder/
        [Authorize]
        [HttpPost("initiateorder")]
        public async Task<ActionResult<CreateOrderResponseDTO>> IntitateOrder(CreateOrderDTO request)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await orderService.CreateOrderAsync(customerId, request);
                var response = new APIResponse<CreateOrderResponseDTO>
                {
                    Success = true,
                    Data = result,
                    Message = "Order created successfully."
                };
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the order.", details = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("orderhistory")]
        public async Task<ActionResult<IEnumerable<OrderItemsResponseDTO>>> GetOrdersByCustomerId()
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await orderService.GetOrdersByCustomerIdAsync(customerId);
                var response = new APIResponse<IEnumerable<OrderItemsResponseDTO>>
                {
                    Success = true,
                    Data = result,
                    Message = result.Any() ? "Orders retrieved successfully of last 5 months." : "No orders found for this customer."
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving orders.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("orderhistory/{year}")]
        public async Task<ActionResult<IEnumerable<OrderItemsResponseDTO>>> GetOrderByYear([FromRoute] int year)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await orderService.GetOrderByYearAsync(customerId, year);
                var response = new APIResponse<IEnumerable<OrderItemsResponseDTO>>
                {
                    Success = true,
                    Data = result,
                    Message = result.Any() ? $"Orders retrieved successfully for the year {year}." : $"No orders found for the year {year}."
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving orders.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("ordersummary")]

        public async Task<ActionResult<OrderSummaryResponseDTO>> GetOrderSummary([FromQuery] Guid addressId)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await orderService.GetOrderSummaryAsync(customerId, addressId);
                var response = new APIResponse<OrderSummaryResponseDTO>
                {
                    Success = true,
                    Data = result,
                    Message = "Order summary retrieved successfully."
                };
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the order summary.", details = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("ordersummarybyOrderId")]
        public async Task<ActionResult<OrderSummaryResponseDTO>> GetOrderSummaryByOrderId([FromQuery] Guid orderId)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await orderService.GetOrderSummaryByOrderIdAsync(customerId, orderId);
                var response = new APIResponse<OrderSummaryResponseDTO>
                {
                    Success = true,
                    Data = result,
                    Message = "Order summary retrieved successfully."
                };
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the order summary.", details = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("detailedview")]
        public async Task<ActionResult<DetailOrderItemViewDTO>> GetDetailedViewofProductbyOrderId([FromQuery] DetailedViewDTO request)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await orderService.GetDetailedViewofProductbyOrderIdAsync(customerId, request);
                var response = new APIResponse<DetailOrderItemViewDTO>
                {
                    Success = true,
                    Data = result,
                    Message = "Detailed view of the product retrieved successfully."
                };
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the detailed view of the product.", details = ex.Message });
            }
        }
    }
}