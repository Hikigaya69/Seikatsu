namespace Seikatsu.Backend.Models
{
    public class CartItemDTO
    {
        public Guid CartItemId { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string ProductImageUrl { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal ItemTotal { get; set; }
    }
}
