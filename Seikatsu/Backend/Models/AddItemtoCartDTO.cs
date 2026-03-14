namespace Seikatsu.Backend.Models
{
    public class AddItemtoCartDTO
    {
        public Guid CartId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal ItemTotal { get; set; }
    }
}
