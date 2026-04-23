using Seikatsu.Backend.Models;
using Seikatsu.Backend.Models.AdminDTOs;
namespace Seikatsu.Backend.Services
{
    public interface IAdminService
    {
         Task<ProductAddResponseDTO> AddProductAsync(ProductAddRequestDTO request);
         Task<ProductDTO>GetProductByIdAsync(Guid productId);

        Task<ProductDTO> EditProductAsync(Guid productId,EditProductRequestDTO request);

        Task <bool>DeleteProductAsync(Guid productId);

        Task<IEnumerable<ProductDTOforIndexPage>> GetProductByCategory(Guid categoryId);

        Task<IEnumerable<ProductCountryDTO>> GetProductbyCountryAsync(string countryname);
    }
}
