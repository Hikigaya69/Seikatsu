using Seikatsu.Backend.Enums;

namespace Seikatsu.Backend.Models
{
    public class RestockCartItemDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }

        public string ProductImageUrl { get; set; } = string.Empty; 
        public int Quantity { get; set; }
     
        public decimal TotalPrice { get; set; }  // ProductPrice * Quantity

        // Frequency
        public RestockFrequency? Frequency { get; set; }  // null = using cart default user didnt choose anything

        public RestockFrequency EffectiveFrequency { get; set; }  // what actually runs (user choose a frequency)

        public RestockItemsStatus? Status { get; set; } //user didnt choose anything so defalut "Live status is set"
        public RestockItemsStatus EffectiveStatus { get; set; } // what actually runs (user choose a status)    


        public DateTime?NextOrderDate { get; set; }
        public DateTime? LastOrderedAt { get; set; }
    }
}
