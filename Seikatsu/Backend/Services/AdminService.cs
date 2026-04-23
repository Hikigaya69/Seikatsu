using Microsoft.EntityFrameworkCore;
using Seikatsu.Backend.Data;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Exceptions;

using Seikatsu.Backend.Models;
using Seikatsu.Backend.Models.AdminDTOs;

namespace Seikatsu.Backend.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserContext _context;
        private readonly IStorageService _storage;

        public AdminService(UserContext context, IStorageService storage)
        {
            _context = context;
            _storage = storage;
        }

        public async Task<ProductAddResponseDTO> AddProductAsync(ProductAddRequestDTO request)
        {

            var imageUrl = await _storage.UploadImageAsync(request.Image, "products");

            //  Create product with the returned URL
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                IsFood = request.IsFood,
                StorageType = request.StorageType,
                ProductImageUrl = imageUrl,        //URL from Supabase, not from request
                CountryName = request.CountryName,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new ProductAddResponseDTO
            {
                Id = product.Id,
                Name = product.Name,
                CategoryId = product.CategoryId,
                Price = product.Price,
                CountryName = product.CountryName,

                CreatedAt = product.CreatedAt
            };
        }

        public async Task<ProductDTO> GetProductByIdAsync(Guid productId)
        {

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new Exception("Product not found.");
            }
            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsFood = product.IsFood,
                StorageType = product.StorageType,
                ProductImageUrl = product.ProductImageUrl,
                CountryName = product.CountryName,
                Category = (await _context.Categories.FindAsync(product.CategoryId))?.CategoryName ?? "Unknown"
            };
        }

        public async Task<ProductDTO> EditProductAsync(Guid productId, EditProductRequestDTO request)
        {

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new NotFoundException("Product not found.");
            }
            var imageUrl = await _storage.UploadImageAsync(request.Image, "products");
            return new ProductDTO
            {
                Id = product.Id,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                IsFood = request.IsFood,
                StorageType = request.StorageType,
                ProductImageUrl = imageUrl,        //URL from Supabase, not from request
                CountryName = request.CountryName,
                Category = (await _context.Categories.FindAsync(request.CategoryId))?.CategoryName ?? "Unknown"
            };



        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new NotFoundException("Product not found.");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<ProductDTOforIndexPage>> GetProductByCategory(Guid categoryId)
        {
            var products = await _context.Products
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
            if (products.Count == 0)
            {
                throw new NotFoundException($"products are not found for category.");
            }
            return products;
        }

        public async Task<IEnumerable<ProductCountryDTO>> GetProductbyCountryAsync(string countryname)
        {
            var products = await _context.Products.
                Where(p => p.CountryName.ToLower() == countryname.ToLower()).
                Select(p => new Models.ProductCountryDTO
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
            if (products.Count == 0)
            {
                throw new NotFoundException($"products are not found for country {countryname}.");
            }
            return products;

        }
    }
}