namespace Seikatsu.Backend.Models
{
    public class UpdateCartResponseDTO
    {
        public Guid CartId { get; set; }
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
        public decimal ItemTotal { get; set; }
        public decimal CartTotal { get; set; }
    }
}
