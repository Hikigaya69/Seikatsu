using Seikatsu.Backend.Enums;

namespace Seikatsu.Backend.Models
{
    public class UpdateRestockCartRequestDTO
    {
        public Guid RestockCartItemId { get; set; }
        public int Quantity { get; set; }
        public RestockFrequency? Frequency { get; set; }  // null means use cart default frequency
    }
}
