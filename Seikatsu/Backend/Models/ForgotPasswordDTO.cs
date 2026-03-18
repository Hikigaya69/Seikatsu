using System.ComponentModel.DataAnnotations;

namespace Seikatsu.Backend.Models
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
