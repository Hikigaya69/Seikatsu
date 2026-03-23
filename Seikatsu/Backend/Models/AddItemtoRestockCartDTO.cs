using Seikatsu.Backend.Enums;

namespace Seikatsu.Backend.Models
{
    public class AddItemtoRestockCartDTO
    {  public Guid RestockCartId { get; set; }   
        public Guid RestockItemId { get; set; }
        public Guid ProductId { get; set; }
        public string Message { get; set; } = string.Empty;


    }
}
