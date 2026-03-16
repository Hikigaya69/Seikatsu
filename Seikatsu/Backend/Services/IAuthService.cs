using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;

namespace Seikatsu.Backend.Services
{
    public interface IAuthService
    {
        Task<CustomerRegisterDTO?> RegisterAsync(CustomerDTO request);
        Task<TokenResponseDto?> LoginAsync(CustomerLoginDTO request);

        Task<bool> LogoutAsync();                    
        Task<TokenResponseDto?> RefreshTokenAsync();
    }
}
