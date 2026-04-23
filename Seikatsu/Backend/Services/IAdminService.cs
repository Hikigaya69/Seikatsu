using Seikatsu.Backend.Models;
using Seikatsu.Backend.Models.AdminDTOs;
namespace Seikatsu.Backend.Services
{
    public interface IAdminService
    {
         Task<ProductAddResponseDTO> AddProductAsync(ProductAddRequestDTO request);
         Task<ProductDTO>GetProductByIdAsync(Guid productId);
//from here
        Task<ProductDTO> EditProductAsync(Guid productId,EditProductRequestDTO request);

        Task <bool>DeleteProductAsync(Guid productId);

        Task<CreateCategoryResponseDTO>CreateCategoryAsync(string categoryName);

        Task<bool>DeleteCategoryAsync(Guid categoryId);
        Task <int>CountCustomersAsync();
        //this for normal,simple dashboard, not for the analytics 
        Task<decimal> TotalRevenueEarnedAsync();
        //this for the analytics dashboard, where the admin can select the date range to see the revenue, this can be converted into graph
        Task<decimal> GetRevenueAsync(DateRequestDTO request);

        //this service will give both total item sold and total orders too
        Task<OrderCountResponseDTO> TotalOrdersAsync();

        Task<OrderCountResponseDTO> TotalOrdersinRangeAsync(DateRequestDTO request);

        Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync();

        Task<IEnumerable<ProductDTOforIndexPage>> GetProductByCategory(Guid categoryId);

        Task<IEnumerable<ProductCountryDTO>> GetProductbyCountryAsync(string countryname);
    }
}
