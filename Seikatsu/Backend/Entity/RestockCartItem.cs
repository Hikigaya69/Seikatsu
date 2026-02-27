namespace Seikatsu.Backend.Entity
{
    public class RestockCartItem
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public Guid RestockCartId { get; set; }
        public RestockCart? RestockCart { get; set; }
    }
}
