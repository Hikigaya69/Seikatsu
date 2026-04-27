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
        [Authorize]
        [HttpGet("orderhistory")]
        public async Task<ActionResult<APIResponse<PagedResult<OrderItemsResponseDTO>>>> GetOrdersByCustomerId(
[FromQuery] int pageSize = 6,
[FromQuery] DateTime? cursorDate = null,
[FromQuery] Guid? cursorId = null)
        {

            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await orderService.GetOrdersByCustomerIdAsync(customerId, pageSize, cursorDate);
            var response = new APIResponse<PagedResult<OrderItemsResponseDTO>>
            {
                Success = true,
                Data = result,
                Message = result.Items.Any() ? "Products are sent." : "No products found."
            };
            return Ok(response);
        }

        
       


        [Authorize]
        [HttpGet("orderhistory/{year}")]

        public async Task<ActionResult<APIResponse<PagedResult<OrderItemsResponseDTO>>>> GetOrderByYear(
            [FromRoute] int year,
      [FromQuery] int pageSize = 6,
       [FromQuery] DateTime? cursorDate = null,
      [FromQuery] Guid? cursorId = null)
        {

            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await orderService.GetOrderByYearAsync(customerId, year,pageSize, cursorDate);
            var response = new APIResponse<PagedResult<OrderItemsResponseDTO>>
            {
                Success = true,
                Data = result,
                Message = result.Items.Any() ? "Products are sent." : "No products found."
            };
            return Ok(response);
        }
    


        [Authorize]
        [HttpGet("ordersummary")]

        public async Task<ActionResult<APIResponse<OrderSummaryResponseDTO>>> GetOrderSummary([FromQuery] Guid addressId)
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


        [Authorize]
        [HttpGet("ordersummarybyOrderId")]
        public async Task<ActionResult<APIResponse<OrderSummaryResponseDTO>>> GetOrderSummaryByOrderId([FromQuery] Guid orderId)
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


        [Authorize]
        [HttpGet("detailedview")]
        public async Task<ActionResult<APIResponse<DetailOrderItemViewDTO>>> GetDetailedViewofProductbyOrderId([FromQuery] DetailedViewDTO request)
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


        [Authorize]
        [HttpPost("verifyorder")]

        public async Task<ActionResult> VerifyPayment(VerifyPaymentDTO request)
        {

            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isPaymentValid = await orderService.VerifyPaymentAsync(customerId, request);
            if (isPaymentValid)
            {
                return Ok(new { message = "Payment verified successfully." });
            }
            else
            {
                return BadRequest(new { message = "Payment verification failed." });
            }
        }


    }
}