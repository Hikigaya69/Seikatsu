namespace Seikatsu.Backend.Entity
{
    public class RestockCart
    {
        public Guid Id { get; set; }

        public string Frequency { get; set; } = string.Empty;

        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public ICollection<RestockCartItem> RestockCartItems { get; set; } = new List<RestockCartItem>();
    }
}
