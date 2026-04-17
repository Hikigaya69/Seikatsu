using Seikatsu.Backend.Enums;

namespace Seikatsu.Backend.Models
{
    public class ItemAddtoRestockCartDTO
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public RestockFrequency Frequency { get; set; }

         // public RestockItemsStatus Status { get; set; }   thjinking
    }
}
