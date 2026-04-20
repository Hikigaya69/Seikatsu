using Seikatsu.Backend.Data;
using Seikatsu.Backend.Entity;

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
    }
}