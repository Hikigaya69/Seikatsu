namespace Seikatsu.Backend.Models
{
    public class AddItemtoCartDTO
    {
        //public Guid ProductId { get; set; }
        public Guid Cartid { get; set; }
        public int? Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public List<CartItemDTO> Items { get; set; } = new();
    }
}
