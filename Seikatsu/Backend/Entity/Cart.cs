namespace Seikatsu.Backend.Entity
{
    public class Cart
    {
        public Guid Id { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
