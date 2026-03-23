using Microsoft.EntityFrameworkCore;
using Seikatsu.Backend.Exceptions;
using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services

{
    public class ProductService(Data.UserContext context) : IProductService
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
            if(products.Count == 0)
            {
                throw new NotFoundException("products are not found.");
            }
            return products;
        }

        public async Task<ProductDTO> GetPrductbyIdAsync(Guid id)
        {
            var product = await context.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    IsFood = p.IsFood,
                    StorageType = p.StorageType,
                    Category = p.Category!.CategoryName,
                    CountryName = p.CountryName
                })
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Product not found.");

            return product;
        }

        public async Task<IEnumerable<Models.ProductCountryDTO>> GetProductbyCountryAsync(string countyname)
        {
            var products = await context.Products.
                Where(p => p.CountryName.ToLower() == countyname.ToLower()).
                Select(p => new Models.ProductCountryDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    IsFood = p.IsFood,
                    ProductImageUrl = p.ProductImageUrl,
                    StorageType = p.StorageType,
                    Category = p.Category!.CategoryName,
                    CountryName = p.CountryName
                }).ToListAsync();
            if(products.Count == 0)
            {
                throw new NotFoundException($"products are not found for  {countyname}.");
            }
            return products;



        }

        public async Task<IEnumerable<Models.ProductSuggestionDTO>> GetProductSuggestionAsync(string query)
        {
            var suggestions = await context.Products
                .Where(p => p.Name.ToLower().Contains(query.ToLower()))
                .Select(p => new Models.ProductSuggestionDTO
                {
                    Id = p.Id,
                    Name = p.Name,

                })
                .Take(8)// Limit the number of suggestions to 8
                .ToListAsync();

            return suggestions;
        }

        public async Task<IEnumerable<Models.ProductDTOforIndexPage>> GetProductsbySearchAsync(string query)
        {
            var products = await context.Products
                .Where(p => p.Name.ToLower().Contains(query.ToLower()))
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

        public async Task<IEnumerable<Models.ProductDTOforIndexPage>> GetProductbyCategoty(Guid categoryId)
        {
            var products = await context.Products
                .Where(p => p.CategoryId == categoryId)
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
            if(products.Count == 0)
            {
                throw new NotFoundException($"products are not found for category.");
            }
            return products;

        }
    }
}