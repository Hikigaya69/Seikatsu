namespace Seikatsu.Backend.Models
{
    public class AddressResponseDTO
    {
        public Guid Id { get; set; }

        // From Customer navigation
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Address fields
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
      //  public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public bool IsDefault { get; set; }
    }
}
