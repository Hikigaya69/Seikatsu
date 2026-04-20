using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Exceptions;
using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public class ChecklistService(Data.UserContext context) : IChecklistService
    {
        public async Task<bool> CreateChecklistAsync(Guid customerId)
        {
            var checklist = new CheckList
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,

            };
            context.CheckLists.Add(checklist);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<AddItemstoChecklistResponseDTO> AddItemstoChecklistAsync(Guid customerId, AddItemstoChecklistRequestDTO request)
        {
            var checklist = await context.CheckLists.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (checklist == null)
            {
                throw new NotFoundException("Checklist not found for the specified customer.");
            }


            var savedItems = request.Items.Select(item => new CheckListItem
            {
                Id = Guid.NewGuid(),
                ProductName = item.ProductName,
                ProductId = item.ProductId, 
                IsChecked = item.IsChecked,
                CheckListId = checklist.Id
            }).ToList();

            await context.CheckListItems.AddRangeAsync(savedItems);
            await context.SaveChangesAsync();

            return new AddItemstoChecklistResponseDTO
            {
                ChecklistId = checklist.Id,
                AddedItems = savedItems.Select(i => new CheckListItemResponseDTO
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    ProductId = i.ProductId,
                    IsChecked = i.IsChecked
                }).ToList()
            };


        }

        public async Task<CheckListItemResponseDTO> AddSingleItemAysnc(Guid customerId, CheckListItemDTO request)
        {

            var checklist = await context.CheckLists.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (checklist == null)
            {
                throw new NotFoundException("Checklist not found for the specified customer.");
            }
            var newItem = new CheckListItem
            {
                Id = Guid.NewGuid(),
                ProductName = request.ProductName,
                IsChecked = request.IsChecked,
                ProductId = request.ProductId,  
                CheckListId = checklist.Id
            };
            context.CheckListItems.Add(newItem);
            await context.SaveChangesAsync();
            return new CheckListItemResponseDTO
            {
                Id = newItem.Id,
                ProductName = newItem.ProductName,
                ProductId = newItem.ProductId, 
                IsChecked = newItem.IsChecked
            };
        }

        public async Task<bool> DeleteItemAsync(Guid customerId, Guid itemId)
        {
            var checklist = await context.CheckLists.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (checklist == null)
            {
                throw new NotFoundException("Checklist not found for the specified customer.");
            }
            var item = await context.CheckListItems.FirstOrDefaultAsync(i => i.Id == itemId && i.CheckListId == checklist.Id);
            if (item == null)
            {
                throw new NotFoundException("Checklist item not found.");
            }
            context.CheckListItems.Remove(item);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCheckListAsync(Guid customerId)
        {
            var checklist = await context.CheckLists.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (checklist == null)
            {
                throw new NotFoundException("Checklist not found for the specified customer.");
            }
            context.CheckLists.Remove(checklist);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<CheckListItemResponseDTO> ModifySingleProductAsync(Guid customerId, Guid itemId, CheckListItemDTO request)
        {
            var checklist = await context.CheckLists.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (checklist == null)
            {
                throw new NotFoundException("Checklist not found for the specified customer.");
            }
            var item = await context.CheckListItems.FirstOrDefaultAsync(i => i.Id == itemId && i.CheckListId == checklist.Id);
            if (item == null)
            {
                throw new NotFoundException("Checklist item not found.");
            }
            item.ProductName = request.ProductName;
            item.IsChecked = request.IsChecked;
            await context.SaveChangesAsync();
            return new CheckListItemResponseDTO
            {
                Id = item.Id,
                ProductName = item.ProductName,
                ProductId = item.ProductId,
                IsChecked = item.IsChecked
            };
        }

        public async Task<ModifyItemsResponseDTO> ModifyProductAsync(Guid customerId, ModifyItemsinChecklistRequestDTO request)
        {
            var checklist = await context.CheckLists.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (checklist == null)
            {
                throw new NotFoundException("Checklist not found for the specified customer.");
            }
            var itemIds = request.Items.Select(i => i.Id).ToList();

            var existingItems = await context.CheckListItems
                .Where(i => i.CheckListId == checklist.Id && itemIds.Contains(i.Id))
                .ToListAsync();

            foreach (var item in existingItems)
            {
                var updatedData = request.Items.First(r => r.Id == item.Id);
                item.ProductName = updatedData.ProductName;
                item.IsChecked = updatedData.IsChecked;
            }

            await context.SaveChangesAsync();

            // existingItems already has the updated values so map directly
            return new ModifyItemsResponseDTO
            {
                ChecklistId = checklist.Id,
                ModifiedItems = existingItems.Select(i => new CheckListItemResponseDTO
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    IsChecked = i.IsChecked
                }).ToList()
            };
        }

        public async Task<GetChecklistDTO> GetCheckListAsync(Guid customerId)
        {
            var checkList = await context.CheckLists
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId)
                ?? throw new NotFoundException("Checklist not found.");

            return new GetChecklistDTO
            {
                ChecklistId = checkList.Id,
                
                Items = checkList.Items.Select(i => new CheckListItemResponseDTO
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    ProductId = i.ProductId,
                    IsChecked = i.IsChecked
                }).ToList()
            };
        }
    }
}