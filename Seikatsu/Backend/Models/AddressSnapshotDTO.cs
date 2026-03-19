namespace Seikatsu.Backend.Models
{
    public class AddressSnapshotDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // where to deliver
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
