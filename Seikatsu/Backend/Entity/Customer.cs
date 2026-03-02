namespace Seikatsu.Backend.Entity
{
    public class Customer
    {
       public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHashed { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<Address> Addresses { get; set; } = new List<Address>();

        public ICollection<Cart> Carts { get; set; } = new List<Cart>();

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public ICollection<CheckList> CheckLists { get; set; } = new List<CheckList>();

        public ICollection<RestockCart> RestockCarts { get; set; } = new List<RestockCart>();
    }
}
