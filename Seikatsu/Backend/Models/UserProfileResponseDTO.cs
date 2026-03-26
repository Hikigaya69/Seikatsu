namespace Seikatsu.Backend.Models
{
    public class UserProfileResponseDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public bool IsDefault { get; set; }
}
}
