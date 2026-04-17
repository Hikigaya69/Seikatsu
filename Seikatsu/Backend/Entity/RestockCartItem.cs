using Seikatsu.Backend.Enums;

namespace Seikatsu.Backend.Entity
{
    public class RestockCartItem
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public Guid RestockCartId { get; set; }
        public RestockCart? RestockCart { get; set; }

        public decimal ProductPrice { get; set; } // Store price at time of adding to cart for consistency

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public RestockFrequency? Frequency { get; set; } = RestockFrequency.Monthly;

        public RestockItemsStatus? Status { get; set; } = RestockItemsStatus.Live;

        public DateTime?NextOrderDate { get; set; }
        public DateTime? LastOrderedAt { get; set; }
    }
}
