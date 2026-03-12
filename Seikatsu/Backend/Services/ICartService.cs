using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface ICartService
    {
        Task<IEnumerable<AddItemtoCartDTO>> AddItemstoCartAysnc(Guid customerid,Guid productid, int quantity);
    }
}
