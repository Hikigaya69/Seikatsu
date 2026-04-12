using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;
using System.Threading.Tasks;

namespace Seikatsu.Backend.Services
{
    public interface IOrderService
    {
       Task<CreateOrderResponseDTO> CreateOrderAsync(Guid customerID,CreateOrderDTO request);
        Task<CreateRestockOrderResponseDTO> CreateRestockOrderAsync(Guid customerId, RestockCartItem item);
        Task<IEnumerable<OrderItemsResponseDTO>> GetOrdersByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<OrderItemsResponseDTO>> GetOrderByYearAsync(Guid customerId,int year);
        Task<OrderSummaryResponseDTO> GetOrderSummaryAsync(Guid customerId, Guid addressId);
        Task<OrderSummaryResponseDTO> GetOrderSummaryByOrderIdAsync(Guid customerId, Guid orderId);

        Task<DetailOrderItemViewDTO> GetDetailedViewofProductbyOrderIdAsync(Guid customerId, DetailedViewDTO request);
        Task<bool> VerifyPaymentAsync(Guid customerId, VerifyPaymentDTO request);
    }
}
