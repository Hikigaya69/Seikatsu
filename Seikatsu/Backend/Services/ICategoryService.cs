using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync();
    }
}
