using Seikatsu.Backend.Models.AdminDTOs;
namespace Seikatsu.Backend.Services
{
    public interface IAdminService
    {
         Task<ProductAddResponseDTO> AddProductAsync(ProductAddRequestDTO request);
    }
}
