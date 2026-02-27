namespace Seikatsu.Backend.Entity
{
    public class CartItem
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public decimal CartTotalPrice { get; set; }

        public Guid CartId { get; set; }
        public Cart? Cart { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
