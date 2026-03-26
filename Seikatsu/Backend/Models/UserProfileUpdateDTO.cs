namespace Seikatsu.Backend.Models
{
    public class UserProfileUpdateDTO
    {
        public string FullName{ get; set; } = string.Empty;
       
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set;} = string.Empty;

        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; }

        public bool IsDefault { get; set; } 
    }
}
