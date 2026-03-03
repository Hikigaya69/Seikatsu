using System.ComponentModel.DataAnnotations;

namespace Seikatsu.Backend.Models
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$",
       ErrorMessage = "Only Gmail addresses allowed.")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
