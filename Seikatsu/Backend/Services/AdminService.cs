using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Seikatsu.Backend.Data;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Enums;
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
        public async Task<RevenueResponseDTO> GetRevenueForPeriodAsync(RevenuePeriodRequestDTO request)
        {

            var now = DateTime.UtcNow;
            DateTime start;
            DateTime end = now;
            string labelFormat;

            switch (request.Period)
            {
                case RevenuePeriod.Today:
                    start = now.Date;
                    end = now;
                    labelFormat = "HH:00";
                    break;

                case RevenuePeriod.Weekly:
                    start = now.Date.AddDays(-6);
                    end = now;
                    labelFormat = "MMM dd";
                    break;

                case RevenuePeriod.Monthly:
                    start = new DateTime(now.Year, now.Month, 1);
                    end = now;
                    labelFormat = "MMM dd";
                    break;

                case RevenuePeriod.Yearly:
                    start = new DateTime(now.Year, 1, 1);
                    end = now;
                    labelFormat = "MMM";
                    break;

                case RevenuePeriod.Custom:
                    if (request.StartDate == null || request.EndDate == null)
                        throw new BadRequestException("StartDate and EndDate are required for Custom period.");

                    if (request.StartDate > request.EndDate)
                        throw new BadRequestException("StartDate cannot be after EndDate.");

                    if (request.EndDate > now)
                        throw new BadRequestException("EndDate cannot be in the future.");

                    start = request.StartDate.Value.Date;
                    end = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                    labelFormat = "MMM dd";
                    break;

                default:
                    throw new BadRequestException("Invalid period.");
            }
            //runs once and stores the result in memory, so that when we use it again for graph plotting, it doesn't hit the database again,
            //it uses the stored result, this is for optimization
            var orders = await _context.Orders
                .Where(o => o.OrderStatus == "Confirmed" && o.CreatedAt >= start && o.CreatedAt <= end
                &&
                (request.CategoryId == null || o.OrderItems.Any(i => i.Product.CategoryId == request.CategoryId)))
                .Select(o => new { o.CreatedAt, o.TotalAmount })
                .ToListAsync();

            if (!orders.Any())
            {
                return new RevenueResponseDTO
                {
                    TotalRevenue = 0,
                    DataPoints = Enumerable.Empty<RevenueDatePointResponseDTO>()
                };
            }
            //list will have unordered data, so we need to order it by date,
            //and then we can group it by date, and then we can sum the total amount for each date,
            //and then we can create a list of RevenueDatePointResponseDTO, this will be used for plotting the graph in the frontend      
            IEnumerable<RevenueDatePointResponseDTO> dataPoints;
            if (request.Period == RevenuePeriod.Today)
            {
                dataPoints = Enumerable.Range(0, 24)
                    .Select(h => new RevenueDatePointResponseDTO
                    {
                        Label = $"{h:D2}:00",
                        Revenue = orders
                            .Where(o => o.CreatedAt.Hour == h)
                            .Sum(o => o.TotalAmount)
                    });
            } //returns based on months
            else if (request.Period == RevenuePeriod.Yearly)
            {
                dataPoints = Enumerable.Range(1, now.Month)  // up to current month only
                    .Select(m => new RevenueDatePointResponseDTO
                    {
                        Label = new DateTime(now.Year, m, 1).ToString("MMM"),
                        Revenue = orders
                            .Where(o => o.CreatedAt.Month == m)
                            .Sum(o => o.TotalAmount)
                    });
            }
            else
            {
                // for Weekly, Monthly, Custom (calulated based on days)
                var totalDays = (int)(end.Date - start.Date).TotalDays + 1;

                dataPoints = Enumerable.Range(0, totalDays)
                    .Select(i =>
                    {
                        var date = start.Date.AddDays(i);
                        return new RevenueDatePointResponseDTO
                        {
                            Label = date.ToString(labelFormat),
                            Revenue = orders
                                .Where(o => o.CreatedAt.Date == date)
                                .Sum(o => o.TotalAmount)
                        };
                    });
            }

            return new RevenueResponseDTO
            {
                TotalRevenue = orders.Sum(o => o.TotalAmount),
                DataPoints = dataPoints
            };

        }
        public async Task<OrderAnalysisResponseDTO> GetOrderForPeriodAsync(RevenuePeriodRequestDTO request)
        {
            var now = DateTime.UtcNow;
            DateTime start;
            DateTime end = now;
            string labelFormat;
            int totalDays = 0;

            switch (request.Period)
            {
                case RevenuePeriod.Today:
                    start = now.Date;
                    end = now;
                    labelFormat = "HH:00";
                    break;

                case RevenuePeriod.Weekly:
                    start = now.Date.AddDays(-6);
                    end = now;
                    labelFormat = "MMM dd";
                    totalDays = 7;
                    break;

                case RevenuePeriod.Monthly:
                    start = new DateTime(now.Year, now.Month, 1);
                    end = now;
                    labelFormat = "MMM dd";
                    totalDays = (int)(end.Date - start.Date).TotalDays + 1;
                    break;

                case RevenuePeriod.Yearly:
                    start = new DateTime(now.Year, 1, 1);
                    end = now;
                    labelFormat = "MMM";
                    break;

                case RevenuePeriod.Custom:
                    if (request.StartDate == null || request.EndDate == null)
                        throw new BadRequestException("StartDate and EndDate are required for Custom period.");

                    if (request.StartDate > request.EndDate)
                        throw new BadRequestException("StartDate cannot be after EndDate.");

                    if (request.EndDate > now)
                        throw new BadRequestException("EndDate cannot be in the future.");

                    start = request.StartDate.Value.Date;
                    end = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                    labelFormat = "MMM dd";
                    break;

                default:
                    throw new BadRequestException("Invalid period.");
            }

            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= start && o.CreatedAt <= end
                &&
                (request.CategoryId == null || o.OrderItems.Any(i => i.Product.CategoryId == request.CategoryId)))
                .Select(o => new { o.CreatedAt, o.OrderStatus })
                .ToListAsync();


            if (!orders.Any())
            {
                return new OrderAnalysisResponseDTO

                {
                    TotalOrders = 0,
                    ConfirmedOrders = 0,
                    PendingOrders = 0,
                    CreatedOrders = 0,
                    DataPoints = Enumerable.Empty<OrderDataPointDTO>()
                };
            }
            //same logic as revenue, but here we are counting the orders based on their status, and then we are creating a list of OrderDataPointDTO,
            //this will be used for plotting the graph in the frontend
            IEnumerable<OrderDataPointDTO> dataPoints;

            if (request.Period == RevenuePeriod.Today)
            {
                dataPoints = Enumerable.Range(0, 24)
                    .Select(h => new OrderDataPointDTO
                    {
                        Label = $"{h:D2}:00",
                        TotalOrders = orders.Where(o => o.CreatedAt.Hour == h).Count(),
                        ConfirmedOrders = orders.Where(o => o.CreatedAt.Hour == h && o.OrderStatus == "Confirmed").Count(),
                        PendingOrders = orders.Where(o => o.CreatedAt.Hour == h && o.OrderStatus == "Pending").Count(),
                        CreatedOrders = orders.Where(o => o.CreatedAt.Hour == h && o.OrderStatus == "Created").Count()
                    });
            }
            else if (request.Period == RevenuePeriod.Yearly)
            {
                dataPoints = Enumerable.Range(1, now.Month)
                    .Select(m => new OrderDataPointDTO
                    {
                        Label = new DateTime(now.Year, m, 1).ToString("MMM"),
                        TotalOrders = orders.Where(o => o.CreatedAt.Month == m).Count(),
                        ConfirmedOrders = orders.Where(o => o.CreatedAt.Month == m && o.OrderStatus == "Confirmed").Count(),
                        PendingOrders = orders.Where(o => o.CreatedAt.Month == m && o.OrderStatus == "Pending").Count(),
                        CreatedOrders = orders.Where(o => o.CreatedAt.Month == m && o.OrderStatus == "Created").Count()
                    });
            }
            else
            {
                // Daily — Weekly, Monthly, Custom
                dataPoints = Enumerable.Range(0, totalDays)
                    .Select(i =>
                    {
                        var date = start.Date.AddDays(i);
                        return new OrderDataPointDTO
                        {
                            Label = date.ToString(labelFormat),
                            TotalOrders = orders.Where(o => o.CreatedAt.Date == date).Count(),
                            ConfirmedOrders = orders.Where(o => o.CreatedAt.Date == date && o.OrderStatus == "Confirmed").Count(),
                            PendingOrders = orders.Where(o => o.CreatedAt.Date == date && o.OrderStatus == "Pending").Count(),
                            CreatedOrders = orders.Where(o => o.CreatedAt.Date == date && o.OrderStatus == "Created").Count()
                        };
                    });
            }

            return new OrderAnalysisResponseDTO
            {
                TotalOrders = orders.Count,
                ConfirmedOrders = orders.Count(o => o.OrderStatus == "Confirmed"),
                PendingOrders = orders.Count(o => o.OrderStatus == "Pending"),
                CreatedOrders = orders.Count(o => o.OrderStatus == "Created"),
                DataPoints = dataPoints
            };


        }

        public async Task<int> CategorywiseProductsSoldAsync(Guid categoryId)
        {
            var count = await _context.OrderItems
                .Where(i => i.Product.CategoryId == categoryId
                         && i.Order.OrderStatus == "Confirmed")
                .SumAsync(i => i.Quantity);  // adds up quantities of all matching items

            return count;
        }

        public async Task<int> CountrywiseProductsSoldAsync(string countryName)
        {
            var count = await _context.OrderItems
                .Where(i => i.Product.CountryName.ToLower() == countryName.ToLower()
                         && i.Order.OrderStatus == "Confirmed")
                .SumAsync(i => i.Quantity);  // adds up quantities of all matching items
            return count;
        }

        public async Task<IEnumerable<CategoryDetailResponseDTO>> ShowCategoryItemCount()
        {

            return await _context.Categories
        .Select(c => new CategoryDetailResponseDTO
        {
            Id = c.Id,
            CategoryName = c.CategoryName,
            ProductCount = c.Products.Count()
        })
        .ToListAsync();
        }

        public async Task<IEnumerable<CountryCountResponseDTO>> ShowCountryItemCount()
        {
            return await _context.Products
                .GroupBy(p => p.CountryName)
                .Select(g => new CountryCountResponseDTO
                {
                    Country = g.Key,
                    ProductCount = g.Count()
                })
                .ToListAsync();

        }
        public async Task<PagedResult<ProductDTOforIndexPage>> GetProductByFilterAdminAsync(
                                      ProductFilterRequestDTO request, int pageSize, DateTime? cursorDate)
        {
            var query = _context.Products.AsQueryable();

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