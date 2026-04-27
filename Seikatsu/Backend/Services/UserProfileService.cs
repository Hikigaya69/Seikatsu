using Microsoft.EntityFrameworkCore;
using Seikatsu.Backend.Exceptions;
using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public class UserProfileService(Data.UserContext context) : IUserProfileService
    {
        public async Task<UserProfileResponseDTO> GetUserDetailsAsync(Guid userId)
        {
            var customer = await context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (customer == null)
            {
                throw new NotFoundException("User not found.");
            }

            var address = customer.Addresses?
                .FirstOrDefault(a => a.IsDefault)
                ?? customer.Addresses.FirstOrDefault();



            return new UserProfileResponseDTO
            {
                Name = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                AddressLine1 = address?.AddressLine1 ?? string.Empty,
                AddressLine2 = address?.AddressLine2 ?? string.Empty,
                City = address?.City ?? string.Empty,
                PostalCode = address?.PostalCode ?? string.Empty,
                Country = address?.Country ?? string.Empty
            };
        }

        public async Task<UserProfileResponseDTO> UpdateUserInfoAsync(Guid userId, UserProfileUpdateDTO request)
        {
            var customer = await context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Id == userId);
            if (customer == null)
            {
                throw new NotFoundException("Customer not found.");
            }

            var address = customer.Addresses
                .FirstOrDefault(a => a.IsDefault)
                ?? customer.Addresses.FirstOrDefault();



            customer.FullName = request.FullName;
            customer.PhoneNumber = request.PhoneNumber;
            customer.Email = request.Email;

            if (address != null)
            {
                address.AddressLine1 = request.AddressLine1;
                address.AddressLine2 = request.AddressLine2;

                address.PostalCode = request.PostalCode;
                address.Country = request.Country;
                address.IsDefault = request.IsDefault;
            }

            await context.SaveChangesAsync();

            return new UserProfileResponseDTO
            {
                Name = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                AddressLine1 = address?.AddressLine1 ?? string.Empty,
                AddressLine2 = address?.AddressLine2,

                PostalCode = address.PostalCode,
                Country = address.Country,
                IsDefault = address.IsDefault


            };
        }

      


        
        public async Task<OrderOverviewDTO> OrderStatusAsync(Guid userId)
        {
            var customer = await context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (customer == null)
            {
                throw new NotFoundException("Customer not found.");

            }
            var result = await context.Orders
                   .Where(o => o.CustomerId == userId)
               .Select(o => new
               {
                   o.TotalAmount,
                   o.OrderStatus,
                   ItemCount = o.OrderItems.Count
               })
                 .ToListAsync();

            var totalOrders = result.Count;

            var totalAmountSpent = result.Sum(x => x.TotalAmount);

            var pendingItems = result
                .Where(x => x.OrderStatus == "Pending")
                .Sum(x => x.ItemCount);

            var shippedItems = result
                .Where(x => x.OrderStatus == "Shipped")
                .Sum(x => x.ItemCount);

            var deliveredItems = result
                .Where(x => x.OrderStatus == "Delivered")
                .Sum(x => x.ItemCount);

            var cancelledItems = result
                .Where(x => x.OrderStatus == "Cancelled")
                .Sum(x => x.ItemCount);

            return new OrderOverviewDTO
            {
                TotalOrders = totalOrders,
                TotalAmountSpent = totalAmountSpent,
                PendingOrders = pendingItems,

                DeliveredOrders = deliveredItems,
                CancelledOrders = cancelledItems
            };

        }
    }
}