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
        public async Task<IEnumerable<AddItemtoCartDTO>> AddItemstoCartAysnc(Guid customerid, Guid productid, int quantity) {

            var pid = await context.Products.FindAsync(productid);
            if (pid == null)
            {
                throw new Exception("Product not found");
            }

            var cart = await context.Carts
                .Include(c=>c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerid);

            if ( cart==null)
            {

                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerid,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Carts.Add(cart);

            }

          var existingItem = cart.CartItems
        .FirstOrDefault(ci => ci.ProductId == productid);

            var price=await context.Products.Where(p=>p.Id==productid).Select(p => p.Price).FirstOrDefaultAsync();

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.CartTotalPrice = existingItem.Quantity * price ;
            }
            else
            {
                var cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = productid,
                    Quantity = quantity,
                    CartTotalPrice =price * quantity
                };

                cart.CartItems.Add(cartItem);
            }
            cart.TotalPrice = cart.CartItems.Sum(ci => ci.CartTotalPrice);
            cart.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return new List<AddItemtoCartDTO>{
  new AddItemtoCartDTO
    {   
        Cartid = cart.Id,
        TotalPrice = cart.TotalPrice,
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
    }
}
