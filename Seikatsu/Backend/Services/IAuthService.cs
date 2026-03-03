using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface IAuthService
    {
        Task<CustomerRegisterDTO?> RegisterAsync(CustomerDTO request);
        Task<TokenResponseDto?> LoginAsync(CustomerDTO request);

        Task<bool>LogoutAsync(Guid userId);     

        Task<TokenResponseDto?> RefreshTokenAsync(RequestTokenRefreshDto request);
    }
}
