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
      
    
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
