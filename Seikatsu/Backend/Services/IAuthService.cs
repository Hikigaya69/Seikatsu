using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;
using System.ComponentModel.DataAnnotations;

namespace Seikatsu.Backend.Services
{
    public interface IAuthService
    {
        Task<CustomerRegisterDTO?> RegisterAsync(CustomerDTO request);
        Task<TokenResponseDto?> LoginAsync(CustomerLoginDTO request);

        Task<bool> LogoutAsync(Guid customerID);
        Task RefreshTokenAsync();

        Task<bool> ForgotPasswordAsync(ForgotPasswordDTO request);
        Task<bool> ResetPasswordAsync(ResetPasswordDTO request);


    }
}
