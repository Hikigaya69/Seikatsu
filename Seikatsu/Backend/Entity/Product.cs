namespace Seikatsu.Backend.Entity
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public bool IsFood { get; set; }

        public string StorageType { get; set; } = string.Empty;

        public string? ProductImageUrl { get; set; }

        public string CountryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
