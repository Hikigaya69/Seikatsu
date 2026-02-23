using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDTO request);
        Task<TokenResponseDto?> LoginAsync(UserDTO request);

        Task<TokenResponseDto?> RefreshTokenAsync(RequestTokenRefreshDto request);
    }
}
