using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface ICartService
    {
        Task<IEnumerable<GetCartDTO>> GetCartAysnc(Guid customerid);
        Task<IEnumerable<AddItemtoCartDTO>> AddItemtoCartAysnc(Guid customerid, ItemAddFieldDTO request);
        Task<bool> DeleteItemFormCartAsync(Guid customerid, Guid cartitemid);
        Task<bool> ClearCartAsync(Guid customerId);
    }
}
