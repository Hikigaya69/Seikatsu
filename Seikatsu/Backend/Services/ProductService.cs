using Microsoft.EntityFrameworkCore;

namespace Seikatsu.Backend.Services

{
    public class ProductService (Data.UserContext context) : IProductService
    {
        public async Task<IEnumerable<Models.ProductDTOforIndexPage>> GetRandomProductsAsync(int count)
        {
            var products = await context.Products
                .OrderBy(p => Guid.NewGuid()) // Randomize the order of products
                .Take(count) // Take the specified number of products
                .Select(p => new Models.ProductDTOforIndexPage
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ProductImageUrl = p.ProductImageUrl,
                    Category = p.Category!.CategoryName,
                    CountryName = p.CountryName
                })
                .ToListAsync();
            return products;
        }
    }
}
