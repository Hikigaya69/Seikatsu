namespace Seikatsu.Backend.Entity
{
    public class Customer
    {
       public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHashed { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string PhoneNumber { get; set; } = string.Empty; 
        public string? RefreshToken { get; set; }

        public string? PasswordResetToken { get; set; }

        public string Role { get; set; } = "customer"; //ddefault role is customer 
        public DateTime? PasswordResetTokenExpiry { get; set; }

        public Cart? Cart { get; set; }

        public RestockCart? RestockCart { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<Address> Addresses { get; set; } = new List<Address>();

       

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public CheckList? CheckList { get; set; }


    }
}
