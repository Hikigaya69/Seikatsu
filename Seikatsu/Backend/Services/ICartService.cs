using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface ICartService
    { 

        Task<bool>CreateCartAsync(Guid customerId); //should automatically run when a new user is registered.
        Task<GetCartDTO> GetCartAysnc(Guid customerid);
        Task<AddItemtoCartDTO> AddItemtoCartAysnc(Guid customerid, ItemAddFieldDTO request);
        Task<bool> DeleteItemFormCartAsync(Guid customerid, Guid cartitemid);
        Task<bool> ClearCartAsync(Guid customerId);
        Task<UpdateCartResponseDTO> UpdateCartItemAysnc(Guid customerid, UpdateCartDTO request);
        Task<CartSummaryDTO> GetCartSummaryAsync(Guid customerId);

    }
}
