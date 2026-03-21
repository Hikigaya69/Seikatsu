namespace Seikatsu.Backend.Models
{
    public class CreateOrderResponseDTO
    {
        public Guid OrderId { get; set; }
        public string RazorpayOrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string KeyId { get; set; } = string.Empty;
    }
}
