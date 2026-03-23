using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Enums;
using Seikatsu.Backend.Exceptions;
using Seikatsu.Backend.Helpers;
using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public class RestockCartService(Data.UserContext context) : IRestockCartService
    {

        public async Task CreateRestockCartAsync(Guid customerId)
        {
            var restockCart = new RestockCart
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
           
                UpdatedAt = DateTime.UtcNow
            };
            context.RestockCarts.Add(restockCart);
            await context.SaveChangesAsync();
            
        }

        public async Task<GetRestockCartDTO> GetRestockCartAsync(Guid customerid)
        {
            var restockCart = await context.RestockCarts
                .Include(rc => rc.RestockCartItems)
                .ThenInclude(rci => rci.Product)
                .FirstOrDefaultAsync(rc => rc.CustomerId == customerid);

            if (restockCart == null)
            {
                throw new NotFoundException("Restock cart not found for the specified customer.");

            }

            return new GetRestockCartDTO
            {
                RestockCartid = restockCart.Id,
               
                TotalItemsCount = restockCart.RestockCartItems.Count,
                TotalPrice = restockCart.RestockCartItems
            .Sum(rci => rci.Product!.Price * rci.Quantity), 
                Items = restockCart.RestockCartItems
            .Select(rci => new RestockCartItemDTO
            {
                Id = rci.Id,
                ProductId = rci.ProductId,
                ProductName = rci.Product!.Name,
                ProductImageUrl = rci.Product.ProductImageUrl,
                ProductPrice = rci.Product.Price,
                Quantity = rci.Quantity,
                TotalPrice = rci.Product.Price * rci.Quantity,
                Frequency = rci.Frequency,    
                NextOrderDate = rci.NextOrderDate,                       
                LastOrderedAt = rci.LastOrderedAt                        
            })
                    .ToList()
            };
         }

        public async Task<AddItemtoRestockCartDTO> AddItemToRestockCartAsync(Guid customerid, ItemAddtoRestockCartDTO request)
        {
            var restockCart = await context.RestockCarts
                .Include(rc => rc.RestockCartItems)
                .ThenInclude(ri=>ri.Product)
                .FirstOrDefaultAsync(rc => rc.CustomerId == customerid);
            if (restockCart == null) { 
            
            throw new NotFoundException("Restock cart not found for the specified customer.");
            }

            var product = await context.Products.FindAsync(request.ProductId);
            if(product == null)
            {
                throw new NotFoundException("Product not found.");
            }
            var existingItem = restockCart.RestockCartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem.ProductPrice = existingItem.Quantity * product.Price;
            }
            else
            {
                var newItem = new RestockCartItem
                {
                    Id = Guid.NewGuid(),
                    RestockCartId = restockCart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Frequency = request.Frequency,
                    NextOrderDate = RestockFrequencyHelper.ComputeNextOrderDate( request.Frequency,DateTime.UtcNow ),
                    LastOrderedAt = null
                };
                context.RestockCartItems.Add(newItem);
                
            }

            restockCart.TotalPrice = restockCart.RestockCartItems.Sum(ci => ci.ProductPrice);
            restockCart.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            return new AddItemtoRestockCartDTO
            {
                RestockCartId = restockCart.Id,
              ProductId = request.ProductId,
             Message= "Item added to restock cart successfully."
            };


        }

        public async Task DeleteItemFormRestockCartAsync(Guid customerid, Guid cartitemid)
        {
            var restockcart = await context.RestockCarts.FirstOrDefaultAsync(c => c.CustomerId == customerid);
            if (restockcart == null)
            {
                throw new NotFoundException("Cart not found for the specified customer.");
            }

            var cartItem = await context.RestockCartItems
           .FirstOrDefaultAsync(ci => ci.Id == cartitemid && ci.RestockCartId == restockcart.Id);

            if (cartItem == null)
            {
                throw new NotFoundException("No item to delete.");
            }
            context.RestockCartItems.Remove(cartItem);

            restockcart.TotalPrice = restockcart.RestockCartItems
        .Where(ci => ci.Id != cartitemid)
        .Sum(ci => ci.ProductPrice);

            restockcart.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            

        }
        public async Task ClearRestockCartAsync(Guid customerId)
        {
             var restockcart = await context.RestockCarts
                .Include(c => c.RestockCartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (restockcart == null)
            {
                throw new NotFoundException("Restock cart not found for the specified customer.");
            }
            context.RestockCartItems.RemoveRange(restockcart.RestockCartItems);
            restockcart.TotalPrice = 0;
            restockcart.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
           
        }

       public async Task<UpdateRestockCartResponseDTO> UpdateRestockCartItemAysnc(Guid customerid, UpdateRestockCartRequestDTO request)
       {

            var restockcart = await context.RestockCarts
                .Include(c => c.RestockCartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerid);

            if (restockcart == null)
            {
                throw new NotFoundException("Cart not found for the specified customer.");
            }

            var cartItem = restockcart.RestockCartItems.FirstOrDefault(ci => ci.Id == request.RestockCartItemId);

            if (cartItem == null)
            {
                throw new NotFoundException("Cart item not found in the customer's cart.");
            }

            if (request.Quantity <= 0)
            {
                context.RestockCartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = request.Quantity;
                cartItem.ProductPrice = cartItem.Product!.Price * request.Quantity;
                cartItem.Frequency = request.Frequency;
                cartItem.NextOrderDate = RestockFrequencyHelper.ComputeNextOrderDate((RestockFrequency)request.Frequency, DateTime.UtcNow);
            }

            restockcart.TotalPrice = restockcart.RestockCartItems.Sum(ci => ci.ProductPrice);
           restockcart.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return new UpdateRestockCartResponseDTO
            {
                RestockCartItemId = cartItem.Id,
                RestockCartId = restockcart.Id,
                Quantity = cartItem.Quantity,
            
                EffectiveFrequency = (RestockFrequency)request.Frequency,
                ItemTotal=cartItem.ProductPrice,
                CartTotal= restockcart.TotalPrice,
                NextOrderDate=cartItem.NextOrderDate
            };


        }

    }
}
