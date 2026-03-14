namespace Seikatsu.Backend.Models
{
    public class GetCartDTO
    {
        //public Guid ProductId { get; set; }
        public Guid Cartid { get; set; }
        public int? TotalItemsCount { get; set; }

        public decimal TotalPrice { get; set; }

        public List<CartItemDTO> Items { get; set; } = new();
    }
}
