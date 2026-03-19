namespace Seikatsu.Backend.Models
{
    public class OrderItemsResponseDTO
    {
        public Guid ProductId { get; set; }    
        public Guid OrderItemId { get; set; }   
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime OrderedDate { get; set; }


        public Guid OrderId { get; set; }            // needed for navigation on click
        public string OrderStatus { get; set; } = string.Empty;

    }
}
