using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
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

        public async Task<CreateCategoryResponseDTO> CreateCategoryAsync(string categoryName)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryName.ToLower());
            if (existingCategory != null)
            {
                throw new ConflictException("Category already exists.");
            }
            var category = new Category
            {
                Id = Guid.NewGuid(),
                CategoryName = categoryName
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CreateCategoryResponseDTO
            {
                Id = category.Id,
                CategoryName = category.CategoryName
            };

        }
        public async Task<bool> DeleteCategoryAsync(Guid categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                throw new NotFoundException("Category not found.");
            }
            var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
            if (hasProducts)
            {
                throw new ConflictException("Cannot delete category with associated products.");
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Models.CategoryResponseDTO>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Select(c => new Models.CategoryResponseDTO
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,

                })
                .ToListAsync();

            if (categories is null)
            {
                throw new NotFoundException("Category not found");
            }
            return categories;
        }
        public async Task<int> CountCustomersAsync()
        {
            var totalCustomersCount = await _context.Customers.CountAsync();

            return totalCustomersCount;
        }

        public async Task<decimal> TotalRevenueEarnedAsync()
        {
            var totalRevenue = await _context.Orders
                .Where(o => o.OrderStatus == "Confirmed")
                .SumAsync(o => o.TotalAmount);
            return totalRevenue;
        }

        public async Task<decimal> GetRevenueAsync(DateRequestDTO request)
        {
            var revenue = await _context.Orders
                .Where(o => o.OrderStatus == "Confirmed" && o.CreatedAt >= request.StartDate && o.CreatedAt <= request.EndDate)
                .SumAsync(o => o.TotalAmount);
            return revenue;
        }

        public async Task<OrderCountResponseDTO> TotalOrdersAsync()
        {
            var orders = await _context.Orders
       .GroupBy(o => o.OrderStatus)
       .Select(g => new { Status = g.Key, Count = g.Count() })
       .ToListAsync();

            return new OrderCountResponseDTO
            {
                TotalOrders = orders.Sum(o => o.Count),
                ConfirmedOrders = orders.FirstOrDefault(o => o.Status == "Confirmed")?.Count ?? 0,
                CreatedOrders = orders.FirstOrDefault(o => o.Status == "Created")?.Count ?? 0,
                PendingOrders = orders.FirstOrDefault(o => o.Status == "Pending")?.Count ?? 0
            };
        }


        public async Task<OrderCountResponseDTO> TotalOrdersinRangeAsync(DateRequestDTO request)
        {
            var orders = await _context.Orders

                .Where(o => o.CreatedAt >= request.StartDate && o.CreatedAt <= request.EndDate)
       .GroupBy(o => o.OrderStatus)
       .Select(g => new { Status = g.Key, Count = g.Count() })
       .ToListAsync();

            return new OrderCountResponseDTO
            {
                TotalOrders = orders.Sum(o => o.Count),
                ConfirmedOrders = orders.FirstOrDefault(o => o.Status == "Confirmed")?.Count ?? 0,
                CreatedOrders = orders.FirstOrDefault(o => o.Status == "Created")?.Count ?? 0,
                PendingOrders = orders.FirstOrDefault(o => o.Status == "Pending")?.Count ?? 0
            };

        }
    }
}