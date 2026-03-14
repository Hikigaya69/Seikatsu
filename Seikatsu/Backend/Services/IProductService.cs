using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTOforIndexPage>> GetRandomProductsAsync(int count);
        Task<IEnumerable<ProductDTO>> GetPrductbyIdAsync(Guid id);

        Task<IEnumerable<ProductCountryDTO>> GetProductbyCountryAsync(string countryname);

        Task<IEnumerable<ProductSuggestionDTO>> GetProductSuggestionAsync(string query);

        Task<IEnumerable<ProductDTOforIndexPage>> GetProductsbySearchAsync(string query);

        Task<IEnumerable<ProductDTOforIndexPage>> GetProductbyCategoty(Guid categoryId);


    }
}
