using Seikatsu.Backend.Enums;

namespace Seikatsu.Backend.Models
{
    public class UpdateRestockCartResponseDTO
    {
        public Guid RestockCartId { get; set; }
        public Guid RestockCartItemId { get; set; }
        public int Quantity { get; set; }
        public RestockFrequency EffectiveFrequency { get; set; }  // what actually runs 
        public decimal ItemTotal { get; set; }
        public decimal CartTotal { get; set; }


        public RestockItemsStatus EffectiveStatus { get; set; } // what actually runs (user choose a status)    

        public DateTime?NextOrderDate { get; set; }
    }
}
