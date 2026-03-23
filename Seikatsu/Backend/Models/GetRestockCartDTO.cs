using Seikatsu.Backend.Enums;

namespace Seikatsu.Backend.Models
{
    public class GetRestockCartDTO
    {
        public Guid RestockCartid { get; set; }
        public int? TotalItemsCount { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime? NearestItemRestockDate { get; set; }
        
        public List<RestockCartItemDTO> Items { get; set; } = new();
    }
}
