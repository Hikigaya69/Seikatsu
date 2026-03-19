namespace Seikatsu.Backend.Models
{
    public class OrderSummaryItemDTO
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
