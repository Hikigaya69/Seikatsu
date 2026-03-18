namespace Seikatsu.Backend.Models
{
    public class UpdateCartDTO
    {

        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }   
    }
}
