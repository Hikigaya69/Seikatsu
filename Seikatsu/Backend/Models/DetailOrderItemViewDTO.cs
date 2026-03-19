using Microsoft.Identity.Client;

namespace Seikatsu.Backend.Models
{
    public class DetailOrderItemViewDTO
    {
        public Guid ProductId { get; set; }
        public Guid OrderId { get; set; }
        public Guid PaymentId { get; set; }

    
        //public string PaymentMethod { get; set; } = string.Empty;

        public string OrderStatus { get; set; } = string.Empty;
        public decimal PriceAtPurchase { get; set; }

        public DateTime OrderedDate { get; set; }
        public string ProductName { get; set; } = string.Empty;
     
        public string ProductImageUrl { get; set; } = string.Empty;

        public decimal SubTotal { get; set; }
        public decimal DeliveryCharge { get; set; }

        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public AddressSnapshotDTO DeliveryAddress { get; set; } = new();
    }

}
