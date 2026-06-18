// Seeders/OrderSeeder.cs
using Bogus;
using Microsoft.EntityFrameworkCore;
using Seikatsu.Backend.Data;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Services;
using System.Text.Json;

public class OrderSeeder(UserContext context, IAddressService addressService, ICartService cartService, IOrderService orderService)
{
    private readonly Random _random = new();

    public async Task SeedAsync()
    {
        // 1. fetch only customers with role == "customer"
        var customers = await context.Customers
            .Where(c => c.Role == "customer")
            .ToListAsync();

        if (!customers.Any())
        {
            Console.WriteLine("⚠️ No customers found with role == Customer.");
            return;
        }

        // 2. fetch all products once
        var products = await context.Products.ToListAsync();
        if (!products.Any())
        {
            Console.WriteLine("⚠️ No products found. Seed products first.");
            return;
        }

        var faker = new Faker("en");
        int seeded = 0;
        int skipped = 0;

        foreach (var customer in customers)
        {
            // --- Address ---
            var existingAddress = await context.Addresses
                .FirstOrDefaultAsync(a => a.CustomerId == customer.Id);

            Guid addressId;

            if (existingAddress != null)
            {
                // skip address seeding, use existing
                addressId = existingAddress.Id;
                Console.WriteLine($"[~] Address already exists for {customer.Email}, skipping address.");
            }
            else
            {
                var address = new Address
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    AddressLine1 = faker.Address.StreetAddress(),
                    AddressLine2 = faker.Address.SecondaryAddress(),
                    City = faker.Address.City(),
                    Country = faker.Address.Country(),
                    PostalCode = faker.Address.ZipCode(),
                    IsDefault = true
                };
                context.Addresses.Add(address);
                await context.SaveChangesAsync();
                addressId = address.Id;
                Console.WriteLine($"[✓] Address seeded for {customer.Email}");
            }

            // --- fetch customer's cart ---
            var cart = await context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customer.Id);

            if (cart == null)
            {
                Console.WriteLine($"[✗] No cart found for {customer.Email}, skipping.");
                skipped++;
                continue;
            }

            // --- seed 3–7 orders per customer across 2024–2026 ---
            int orderCount = _random.Next(3, 8);

            for (int o = 0; o < orderCount; o++)
            {
                // clear cart items for each order
                var existingCartItems = await context.CartItems
                    .Where(ci => ci.CartId == cart.Id)
                    .ToListAsync();
                context.CartItems.RemoveRange(existingCartItems);
                await context.SaveChangesAsync();

                // add 1–5 random products to cart
                int itemCount = _random.Next(1, 6);
                var selectedProducts = products
                    .OrderBy(_ => _random.Next())
                    .Take(itemCount)
                    .ToList();

                var cartItems = new List<CartItem>();
                foreach (var product in selectedProducts)
                {
                    int qty = _random.Next(1, 4);
                    cartItems.Add(new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        ProductId = product.Id,
                        Quantity = qty,
                        CartTotalPrice = product.Price * qty
                    });
                }

                context.CartItems.AddRange(cartItems);
                cart.TotalPrice = cartItems.Sum(ci => ci.CartTotalPrice);
                cart.UpdatedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();

                // --- build address snapshot ---
                var addressEntity = await context.Addresses
                    .Include(a => a.Customer)
                    .FirstAsync(a => a.Id == addressId);

                var addressSnapshot = JsonSerializer.Serialize(new
                {
                    FullName = customer.FullName,
                    PhoneNumber = customer.PhoneNumber,
                    AddressLine1 = addressEntity.AddressLine1,
                    AddressLine2 = addressEntity.AddressLine2,
                    City = addressEntity.City,
                    Country = addressEntity.Country,
                    PostalCode = addressEntity.PostalCode
                });

                // --- calculate totals (same logic as OrderService) ---
                var subTotal = cartItems.Sum(ci => ci.Quantity * ci.CartTotalPrice / ci.Quantity); // price * qty
                var deliveryCharge = subTotal > 500 ? 0m : 49m;
                var tax = Math.Round(subTotal * 0.18m, 2);
                var grandTotal = subTotal + deliveryCharge + tax;

                // --- random date between 2024-01-01 and 2026-05-30 ---
                var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var endDate = new DateTime(2026, 5, 30, 0, 0, 0, DateTimeKind.Utc);
                var randomDate = startDate.AddDays(_random.Next((endDate - startDate).Days));

                // --- bogus Razorpay IDs ---
                var razorpayOrderId = $"order_{faker.Random.AlphaNumeric(16)}";
                var razorpayPaymentId = $"pay_{faker.Random.AlphaNumeric(16)}";

                var orderId = Guid.NewGuid();

                var orderItems = cartItems.Select(ci => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    PriceAtPurchase = ci.CartTotalPrice / ci.Quantity
                }).ToList();

                var order = new Order
                {
                    Id = orderId,
                    CustomerId = customer.Id,
                    AddressSnapshot = addressSnapshot,
                    TotalAmount = grandTotal,
                    OrderStatus = "Delivered",   // seeded orders are completed
                    CreatedAt = randomDate,
                    OrderItems = orderItems
                };

                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    RazorpayOrderId = razorpayOrderId,
                    RazorpayPaymentId = razorpayPaymentId,
                    Amount = grandTotal,
                    Currency = "INR",
                    Status = "Captured",   // seeded payments are paid
                    CreatedAt = randomDate
                };

                await using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    context.Orders.Add(order);
                    context.Payments.Add(payment);
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    seeded++;
                    Console.WriteLine($"[✓] Order {o + 1}/{orderCount} seeded for {customer.Email} ({randomDate:yyyy-MM-dd})");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"[✗] Order failed for {customer.Email}: {ex.Message}");
                    skipped++;
                }
            }
        }

        Console.WriteLine($"\n✅ Done! {seeded} orders seeded, {skipped} skipped.");
    }
}