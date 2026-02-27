namespace Seikatsu.Backend.Entity
{
    public class Payment
    {
        public Guid Id { get; set; }

        // Internal Order
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        // Razorpay
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpaySignature { get; set; }

        // Transaction Details
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";

        // Created | Authorized | Captured | Failed | Refunded
        public string Status { get; set; } = "Created";

        public string? FailureReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
    }
}
