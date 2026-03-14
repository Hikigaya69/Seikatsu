using Microsoft.EntityFrameworkCore;
namespace Seikatsu.Backend.Services
{
    public class CategoryService(Data.UserContext context):ICategoryService
    {
      public async Task<IEnumerable<Models.CategoryResponseDTO>> GetAllCategoriesAsync()
        {
            var categories = await context.Categories
                .Select(c => new Models.CategoryResponseDTO
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    
                })
                .ToListAsync();
            return categories;
        }
    }
}
