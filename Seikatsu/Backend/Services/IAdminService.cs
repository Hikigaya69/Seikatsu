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

        //this for plotting graphs.. If categoryId can be null also.. if choosen the analysis will be for that perticular category
        Task<RevenueResponseDTO> GetRevenueForPeriodAsync(RevenuePeriodRequestDTO request);

        //this for plotting graphs... here ddont mind the request dto name.. fileds are same
        Task<OrderAnalysisResponseDTO> GetOrderForPeriodAsync(RevenuePeriodRequestDTO request);

        //for single category sales count.. suitable for tile in dashboard
        Task<int>CategorywiseProductsSoldAsync(Guid categoryId);

        Task<int> CountrywiseProductsSoldAsync(string countryName);

        Task<IEnumerable<CategoryDetailResponseDTO>> ShowCategoryItemCount();
       Task<IEnumerable<CountryCountResponseDTO>> ShowCountryItemCount();

        
    }
}
