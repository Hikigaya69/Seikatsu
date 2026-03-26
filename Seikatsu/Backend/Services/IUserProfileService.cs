using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface IUserProfileService
    {
        Task<UserProfileResponseDTO> GetUserDetailsAsync(Guid userId);

        Task<UserProfileResponseDTO> UpdateUserInfoAsync(Guid userId, UserProfileUpdateDTO request);

        Task<OrderOverviewDTO> OrderStatusAsync(Guid userId);
    }
}
