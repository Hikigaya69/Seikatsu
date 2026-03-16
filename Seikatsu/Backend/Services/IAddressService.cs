
using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface IAddressService
    {
        public Task<IEnumerable<AddressResponseDTO>> GetAllAddressAysnc(Guid customerID);
        public Task<IEnumerable<AddressResponseDTO>> GetDefaultAddressAysnc(Guid customerID);

        public Task<AddressResponseDTO> AddAddressAsync(Guid customerID,AddAddressDTO request);

        public Task<bool>DeleteAddressAsync(Guid customerID, Guid addressID);

        public Task<UpdateAddressDTO> UpdateAddressAsync(Guid customerID, UpdateAddressDTO request, Guid addressID);

    }
}
