using Seikatsu.Backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Seikatsu.Backend.Services

{
    public class CartService(Data.UserContext context) : ICartService
    {
        public async Task<IEnumerable<GetCartDTO>> GetCartAysnc(Guid customerid)
        {

            var cart = await context.Carts
        .Include(c => c.CartItems)
        .ThenInclude(ci => ci.Product)
        .FirstOrDefaultAsync(c => c.CustomerId == customerid);

            if (cart == null)
            {
                return new List<GetCartDTO>();
            }

            return new List<GetCartDTO>
    {
        new GetCartDTO
        {
            Cartid = cart.Id,
            TotalPrice = cart.CartItems.Sum(ci => ci.CartTotalPrice),
            Items = cart.CartItems
                .Select(ci => new CartItemDTO
                {
                    CartItemId = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product!.Name,
                    ProductImageUrl = ci.Product.ProductImageUrl,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity,
                    ItemTotal = ci.CartTotalPrice
                })
                .ToList()
        }
    };

        }
        public async Task<IEnumerable<AddItemtoCartDTO>> AddItemtoCartAysnc(Guid customerId, ItemAddFieldDTO request)
        {
            var product = await context.Products.FindAsync(request.ProductId);
            var cart = await context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    CartItems = new List<CartItem>()
                };
                context.Carts.Add(cart);
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem.CartTotalPrice = existingItem.Quantity * product.Price;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    CartTotalPrice = product.Price * request.Quantity
                });
            }

            cart.TotalPrice = cart.CartItems.Sum(ci => ci.CartTotalPrice);
            cart.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            var addedItem = cart.CartItems.First(ci => ci.ProductId == request.ProductId);

            return new List<AddItemtoCartDTO>
    {
        new AddItemtoCartDTO
        {
            CartId = cart.Id,
            CustomerId = customerId,
            ProductId = request.ProductId,
            ProductName = product!.Name,
            Quantity = addedItem.Quantity,
            ItemTotal = addedItem.CartTotalPrice
        }
    };
        }

        public async Task<bool> DeleteItemFormCartAsync(Guid customerid, Guid cartitemid)
        {
            var cart = await context.Carts.FirstOrDefaultAsync(c => c.CustomerId == customerid);
            if (cart == null)
            {
                return false;
            }

            var cartItem = await context.CartItems
           .FirstOrDefaultAsync(ci => ci.Id == cartitemid && ci.CartId == cart.Id);

            if (cartItem == null)
            { return false;
            }


            context.CartItems.Remove(cartItem);


            cart.TotalPrice = cart.CartItems
        .Where(ci => ci.Id != cartitemid)
        .Sum(ci => ci.CartTotalPrice);

            cart.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> ClearCartAsync(Guid customerId)
        {
            var cart = await context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
                return false;

            context.CartItems.RemoveRange(cart.CartItems);
            cart.TotalPrice = 0;
            cart.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return true;
        }

    }
}
