using Seikatsu.Backend.Enums;

namespace Seikatsu.Backend.Models
{
    public class RestockItemStatusRequestDTO
    {
       public Guid RestockCartItemId { get; set; }
        public Guid RestockCartId { get; set; }
       public RestockItemsStatus? Status { get; set; }
    }
}
