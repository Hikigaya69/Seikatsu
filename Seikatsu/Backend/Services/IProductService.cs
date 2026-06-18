using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDTOforIndexPage>> GetRandomProductsAsync(
      int pageSize, DateTime? cursorDate);
        Task<ProductDTO> GetPrductbyIdAsync(Guid id);

        Task<IEnumerable<ProductCountryDTO>> GetProductbyCountryAsync(string countryname);
        Task<IEnumerable<CountryDTO>> GetAllCountryAsync();

        Task<IEnumerable<ProductSuggestionDTO>> GetProductSuggestionAsync(string query);

        Task<IEnumerable<ProductDTOforIndexPage>> GetProductsbySearchAsync(string query);

        Task<PagedResult<ProductDTOforIndexPage>> GetProductByCategoryAsync(Guid categoryId, int pageSize, DateTime? cursorDate);
        Task<PagedResult<ProductDTOforIndexPage>> GetProductByFilterAsync(ProductFilterRequestDTO request, int pageSize, DateTime? cursorDate);


    }
}
