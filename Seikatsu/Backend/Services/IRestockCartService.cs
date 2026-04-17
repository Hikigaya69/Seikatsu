using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface IRestockCartService
    {
        Task CreateRestockCartAsync(Guid customerId);
        Task<GetRestockCartDTO> GetRestockCartAsync(Guid customerid);

        Task<AddItemtoRestockCartDTO>AddItemToRestockCartAsync(Guid customerid, ItemAddtoRestockCartDTO request);

        Task DeleteItemFormRestockCartAsync(Guid customerid, Guid restockCartitemid);
        Task ClearRestockCartAsync(Guid customerId);
        Task ProcessRestockOrdersAsync();

        Task <UpdateRestockCartStatusResponseDTO> SetRestockItemStatusAsync(Guid customerid, RestockItemStatusRequestDTO request);
        Task<UpdateRestockCartResponseDTO> UpdateRestockCartItemAysnc(Guid customerid, UpdateRestockCartRequestDTO request);
    }
}
