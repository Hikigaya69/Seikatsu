using Microsoft.EntityFrameworkCore;
using Seikatsu.Backend.Exceptions;
using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services

{
    public class ProductService(Data.UserContext context) : IProductService
    {
        public async Task<PagedResult<ProductDTOforIndexPage>> GetRandomProductsAsync(
    int pageSize, DateTime? cursorDate)
        {
            var query = context.Products
                .OrderBy(p => p.CreatedAt)
                .ThenBy(p => p.Id);


            if (cursorDate != null)
                query = context.Products
                    .Where(p => p.CreatedAt > cursorDate)
                    .OrderBy(p => p.CreatedAt)
                    .ThenBy(p => p.Id);
            var rows = await query
                .Take(pageSize + 1)
                .Select(p => new ProductDTOforIndexPage
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ProductImageUrl = p.ProductImageUrl,
                    Category = p.Category!.CategoryName,
                    CountryName = p.CountryName,
                    CreatedAt = p.CreatedAt  // needed to set next cursor
                })
                .ToListAsync();

            if (rows.Count == 0 && cursorDate == null)
                throw new NotFoundException("No products found.");

            var hasMore = rows.Count > pageSize;
            var items = hasMore ? rows.Take(pageSize).ToList() : rows;

            return new PagedResult<ProductDTOforIndexPage>
            {
                Items = items,
                NextCursorDate = hasMore ? items.Last().CreatedAt : null,

            };
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
                    ProductImageUrl = p.ProductImageUrl,
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
            if (products.Count == 0)
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

        public async Task<PagedResult<ProductDTOforIndexPage>> GetProductByCategoryAsync(
      Guid categoryId, int pageSize, DateTime? cursorDate)
        {
            var query = context.Products
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.CreatedAt)
                .ThenBy(p => p.Id);

            if (cursorDate != null)
                query = context.Products
                    .Where(p => p.CategoryId == categoryId)
                    .Where(p => p.CreatedAt > cursorDate)
                    .OrderBy(p => p.CreatedAt)
                    .ThenBy(p => p.Id);

            var rows = await query
                .Take(pageSize + 1)
                .Select(p => new ProductDTOforIndexPage
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ProductImageUrl = p.ProductImageUrl,
                    Category = p.Category!.CategoryName,
                    CountryName = p.CountryName,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            if (rows.Count == 0 && cursorDate == null)
                throw new NotFoundException("No products found for this category.");

            var hasMore = rows.Count > pageSize;
            var items = hasMore ? rows.Take(pageSize).ToList() : rows;

            return new PagedResult<ProductDTOforIndexPage>
            {
                Items = items,
                NextCursorDate = hasMore ? items.Last().CreatedAt : null
            };
        }

        public async Task<PagedResult<ProductDTOforIndexPage>> GetProductByFilterAsync(
                                      ProductFilterRequestDTO request, int pageSize, DateTime? cursorDate)
        {
            var query = context.Products.AsQueryable();

            // filters
            if (request.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);

            if (!string.IsNullOrEmpty(request.CountryName))
                query = query.Where(p => p.CountryName == request.CountryName);

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= request.MaxPrice.Value);

            if (!string.IsNullOrEmpty(request.StorageType))
                query = query.Where(p => p.StorageType == request.StorageType);

            // cursor — just another chained Where
            if (cursorDate.HasValue)
                query = query.Where(p => p.CreatedAt > cursorDate.Value);

            var rows = await query
                .OrderBy(p => p.CreatedAt)
                .ThenBy(p => p.Id)
                .Take(pageSize + 1)
                .Select(p => new ProductDTOforIndexPage
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ProductImageUrl = p.ProductImageUrl,
                    Category = p.Category!.CategoryName,
                    CountryName = p.CountryName,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            if (rows.Count == 0 && !cursorDate.HasValue)
                throw new NotFoundException("No products found.");

            var hasMore = rows.Count > pageSize;
            var items = hasMore ? rows.Take(pageSize).ToList() : rows;

            return new PagedResult<ProductDTOforIndexPage>
            {
                Items = items,
                NextCursorDate = hasMore ? items.Last().CreatedAt : null
            };
        }
    }
}