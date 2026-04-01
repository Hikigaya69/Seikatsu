using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface IChecklistService
    {
        Task<bool> CreateChecklistAsync(Guid customerId);

        Task<AddItemstoChecklistResponseDTO>AddItemstoChecklistAsync(Guid customerId, AddItemstoChecklistRequestDTO request);

        Task<CheckListItemResponseDTO> AddSingleItemAysnc(Guid customerId, CheckListItemDTO request);

        Task<bool>DeleteItemAsync(Guid customerId, Guid itemId);
        Task<bool>DeleteCheckListAsync(Guid customerId);

        Task<CheckListItemResponseDTO> ModifySingleProductAsync(Guid customerId, Guid itemId, CheckListItemDTO request);
        Task<ModifyItemsResponseDTO> ModifyProductAsync(Guid customerId, ModifyItemsinChecklistRequestDTO request);

        Task<GetChecklistDTO> GetCheckListAsync(Guid customerId);

    }
}
