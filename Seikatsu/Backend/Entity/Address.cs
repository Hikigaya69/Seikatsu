namespace Seikatsu.Backend.Entity
{
    public class Address
    {
        public Guid Id { get; set; }

        public string AddressLine1 { get; set; } = string.Empty;

        public string? AddressLine2 { get; set; }

        public string City { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;

        public bool IsDefault { get; set; }

        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }
}
