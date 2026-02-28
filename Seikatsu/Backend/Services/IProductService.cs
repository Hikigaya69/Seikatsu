using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTOforIndexPage>> GetRandomProductsAsync(int count);
    }
}
