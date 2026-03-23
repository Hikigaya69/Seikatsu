using Seikatsu.Backend.Enums;

namespace Seikatsu.Backend.Entity
{
    public class RestockCart
    {
        public Guid Id { get; set; }
        public decimal TotalPrice { get; set; }
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RestockCartItem> RestockCartItems { get; set; } = new List<RestockCartItem>();
    }
}
