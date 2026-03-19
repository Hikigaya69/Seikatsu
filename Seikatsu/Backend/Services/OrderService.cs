using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace Seikatsu.Backend.Services
{
    public class OrderService(Data.UserContext context) : IOrderService
    {
        public async Task<CreateOrderResponseDTO> CreateOrderAsync(Guid customerId, CreateOrderDTO request)
        {

            var cart = await context.Carts
                 .Include(c => c.CartItems)
                 .ThenInclude(ci => ci.Product)
                 .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                throw new KeyNotFoundException("Cart not found.");
            }

            if (!cart.CartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty.");
            }

            var address = await context.Addresses
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == request.AddressId
                                   && a.CustomerId == customerId);

            if (address == null)
                throw new KeyNotFoundException("Address not found.");

            var addressSnapshot = JsonSerializer.Serialize(new AddressSnapshotDTO
            {
                FullName = address.Customer!.FullName,
                PhoneNumber = address.Customer!.PhoneNumber,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                Country = address.Country,
                PostalCode = address.PostalCode
            });


            var orderId = Guid.NewGuid();
            var orderItems = cart.CartItems.Select(ci => new Entity.OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                PriceAtPurchase = ci.Product!.Price
            }).ToList();

            var order = new Order
            {
                Id = orderId,
                CustomerId = customerId,
                AddressSnapshot = addressSnapshot,
                TotalAmount = orderItems.Sum(i => i.Quantity * i.PriceAtPurchase),
                OrderStatus = "Pending",
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems



            };

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.Orders.Add(order);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return new CreateOrderResponseDTO
            {
                OrderId = order.Id
            };


        }
        //get all orders of a customer in last 5 months, sorted by order date desc. This will be used in order history page.
        //can also add pagination later if needed.
        public async Task<IEnumerable<OrderItemsResponseDTO>> GetOrdersByCustomerIdAsync(Guid customerId)
        {
            var fiveMonthsAgo = DateTime.UtcNow.AddMonths(-5);

            var orders = await context.Orders
                .Where(o => o.CustomerId == customerId && o.CreatedAt >= fiveMonthsAgo)
                .OrderByDescending(o => o.CreatedAt)
                .SelectMany(o => o.OrderItems, (o, oi) => new OrderItemsResponseDTO
                {
                    OrderId = o.Id,
                    OrderItemId = oi.Id,
                    OrderStatus = o.OrderStatus,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product!.Name,
                    ProductImageUrl = oi.Product!.ProductImageUrl,
                    Quantity = oi.Quantity,
                    OrderedDate = o.CreatedAt
                })
                .ToListAsync();
            return orders;
        }
        //get all orders of a customer in a specific year, sorted by order date desc.
        //This will be used in order history page when user clicks on a specific year.
        public async Task<IEnumerable<OrderItemsResponseDTO>> GetOrderByYearAsync(Guid customerId,int year)
        {
            

            var orders = await context.Orders
                .Where(o => o.CustomerId == customerId && o.CreatedAt.Year == year)
                .OrderByDescending(o => o.CreatedAt)
                .SelectMany(o => o.OrderItems, (o, oi) => new OrderItemsResponseDTO
                {
                    OrderId = o.Id,
                    OrderItemId = oi.Id,
                    OrderStatus = o.OrderStatus,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product!.Name,
                    ProductImageUrl = oi.Product!.ProductImageUrl,
                    Quantity = oi.Quantity,
                    OrderedDate = o.CreatedAt
                })
                .ToListAsync();
            return orders;
        }
        //before placing the order, we can show the summary of the order to the user,
        //so that they can confirm before placing the order. This will be based on the cart and address they have selected.
        public async Task<OrderSummaryResponseDTO> GetOrderSummaryAsync(Guid customerId, Guid addressId)
        {
            // 1. fetch cart
            var cart = await context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
                throw new KeyNotFoundException("Cart not found.");

            if (!cart.CartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

            // 2. fetch address
            var address = await context.Addresses
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == addressId
                                       && a.CustomerId == customerId);

            if (address == null)
                throw new KeyNotFoundException("Address not found.");

            // 3. build items
            var items = cart.CartItems.Select(ci => new OrderSummaryItemDTO
            {
                ProductId = ci.ProductId,
                ProductName = ci.Product!.Name,
                ProductImageUrl = ci.Product!.ProductImageUrl,
                Quantity = ci.Quantity,
                UnitPrice = ci.Product!.Price,
                LineTotal = ci.Quantity * ci.Product!.Price
            }).ToList();

            // 4. bill split
            var subTotal = items.Sum(i => i.LineTotal);
            var deliveryCharge = subTotal > 500 ? 0m : 49m;
            var tax = Math.Round(subTotal * 0.18m, 2);

            return new OrderSummaryResponseDTO
            {
                DeliveryAddress = new AddressSnapshotDTO
                {
                    FullName = address.Customer!.FullName,
                    PhoneNumber = address.Customer!.PhoneNumber,
                    AddressLine1 = address.AddressLine1,
                    AddressLine2 = address.AddressLine2,
                    City = address.City,
                    Country = address.Country,
                    PostalCode = address.PostalCode
                },
                Items = items,
                SubTotal = subTotal,
                DeliveryCharge = deliveryCharge,
                
                Tax = tax,
                TotalAmount = subTotal + deliveryCharge  + tax,
                EstimatedDelivery = DateTime.UtcNow.AddDays(5)
            };
        }
        //after placing the order, user can see the summary of the order based on the orderId. This will read from the saved order and not from the cart,
        //as the cart might have changed after placing the order.
        public async Task<OrderSummaryResponseDTO> GetOrderSummaryByOrderIdAsync(Guid customerId, Guid orderId)
        {
            // reads from saved order — not cart
            var order = await context.Orders
                .Where(o => o.Id == orderId && o.CustomerId == customerId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync();

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            var items = order.OrderItems.Select(oi => new OrderSummaryItemDTO
            {
                ProductId = oi.ProductId,
                ProductName = oi.Product!.Name,
                ProductImageUrl = oi.Product!.ProductImageUrl,
                Quantity = oi.Quantity,
                UnitPrice = oi.PriceAtPurchase,   // use snapshotted price
                LineTotal = oi.Quantity * oi.PriceAtPurchase
            }).ToList();

            var subTotal = items.Sum(i => i.LineTotal);
            var deliveryCharge = subTotal > 500 ? 0m : 49m;
      
            var tax = Math.Round(subTotal * 0.18m, 2);
            var payment = order.Payments.OrderByDescending(p => p.CreatedAt).FirstOrDefault();

            return new OrderSummaryResponseDTO
            {
                DeliveryAddress = JsonSerializer.Deserialize<AddressSnapshotDTO>(order.AddressSnapshot)!,
                Items = items,
                SubTotal = subTotal,
                DeliveryCharge = deliveryCharge,
                Tax = tax,
                TotalAmount = order.TotalAmount,  // use saved total, not recalculated
                OrderStatus = order.OrderStatus,
                PaymentStatus = payment?.Status ?? "Unpaid",
                EstimatedDelivery = order.CreatedAt.AddDays(5)
            };
        }

        public async Task<DetailOrderItemViewDTO> GetDetailedViewofProductbyOrderIdAsync(Guid customerId, DetailedViewDTO request)
        {
            var orderItem= await context.OrderItems.Where(oi=>oi.Id==request.OrderItemId && oi.OrderId == request.OrderId &&
            oi.Order!.CustomerId == customerId)
                .Include(oi=>oi.Product)
                .Include(oi=>oi.Order)
                    .ThenInclude(o=>o.Payments)
                .FirstOrDefaultAsync();

            if(orderItem == null)
            {
                throw new KeyNotFoundException("Order item not found.");
            }

            var order= orderItem.Order!;
            var payment = order.Payments.FirstOrDefault();


            var deliveryAddress = string.IsNullOrWhiteSpace(order.AddressSnapshot)
                ? new AddressSnapshotDTO()
                : JsonSerializer.Deserialize<AddressSnapshotDTO>(order.AddressSnapshot)
                  ?? new AddressSnapshotDTO();
            return new DetailOrderItemViewDTO
            {
                ProductId = orderItem.ProductId,
                OrderId = orderItem.OrderId,
                PaymentId = payment?.Id ?? Guid.Empty,
                OrderStatus = order.OrderStatus,
                PriceAtPurchase = orderItem.PriceAtPurchase,
                OrderedDate = order.CreatedAt,
                ProductName = orderItem.Product!.Name,
                ProductImageUrl = orderItem.Product!.ProductImageUrl,   
                DeliveryAddress = deliveryAddress 

            };
        }
    }
}