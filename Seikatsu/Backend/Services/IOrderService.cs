using Seikatsu.Backend.Models;
using System.Threading.Tasks;

namespace Seikatsu.Backend.Services
{
    public interface IOrderService
    {
       Task<CreateOrderResponseDTO> CreateOrderAsync(Guid customerID,CreateOrderDTO request);
        Task<IEnumerable<OrderItemsResponseDTO>> GetOrdersByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<OrderItemsResponseDTO>> GetOrderByYearAsync(Guid customerId,int year);
        Task<OrderSummaryResponseDTO> GetOrderSummaryAsync(Guid customerId, Guid addressId);
        Task<OrderSummaryResponseDTO> GetOrderSummaryByOrderIdAsync(Guid customerId, Guid orderId);

        Task<DetailOrderItemViewDTO> GetDetailedViewofProductbyOrderIdAsync(Guid customerId, DetailedViewDTO request);
    }
}
