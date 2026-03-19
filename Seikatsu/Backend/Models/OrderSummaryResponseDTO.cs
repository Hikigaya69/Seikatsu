namespace Seikatsu.Backend.Models
{
    public class OrderSummaryResponseDTO
    {
        public AddressSnapshotDTO DeliveryAddress { get; set; } = new();
        public List<OrderSummaryItemDTO> Items { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal DeliveryCharge { get; set; }
       
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; } 
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime EstimatedDelivery { get; set; }
    }
}
