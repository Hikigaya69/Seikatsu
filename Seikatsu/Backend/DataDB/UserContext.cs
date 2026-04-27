using Seikatsu.Backend.Entity;
using Microsoft.EntityFrameworkCore;

namespace Seikatsu.Backend.Data
{
    public class UserContext:DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
            //necessary constructor to pass options to the base DbContext class
        }
        // this is mandotary to override the OnModelCreating method
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            // Category → Product (1:M)
            
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Product>()
    .HasIndex(p => new { p.CreatedAt, p.Id });

            // Customer → Address (1:M)

            modelBuilder.Entity<Address>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);


            // Customer → Cart (1:M)

            modelBuilder.Entity<Cart>()
       .HasOne(c => c.Customer)
       .WithOne(cu => cu.Cart)
       .HasForeignKey<Cart>(c => c.CustomerId)
       .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.CustomerId)
                .IsUnique();


            // Cart → CartItem (1:M)

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

           
            // Product → CartItem (1:M)
          
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer → Order (1:M)
           
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

          
            // Order → OrderItem (1:M)
           
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

           
            // Product → OrderItem (1:M)
           
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → Payments (1:M)  Razorpay
            
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Razorpay unique order id
            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.RazorpayOrderId)
                .IsUnique();

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.RazorpayPaymentId);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<CheckList>()
                 .HasOne(cl => cl.Customer)
                 .WithOne(c => c.CheckList)
                 .HasForeignKey<CheckList>(cl => cl.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);

            // CheckList → CheckListItem (1:M)
            modelBuilder.Entity<CheckListItem>()
                .HasOne(i => i.CheckList)
                .WithMany(cl => cl.Items)
                .HasForeignKey(i => i.CheckListId)
                .OnDelete(DeleteBehavior.Cascade);


            // Customer → RestockCart (1:M)

            modelBuilder.Entity<RestockCart>()
    .HasOne(rc => rc.Customer)
    .WithOne(c => c.RestockCart)
    .HasForeignKey<RestockCart>(rc => rc.CustomerId)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RestockCart>()
    .HasIndex(rc => rc.CustomerId)
    .IsUnique();

            // RestockCart → RestockCartItem (1:M)

            modelBuilder.Entity<RestockCartItem>()
                .HasOne(rci => rci.RestockCart)
                .WithMany(rc => rc.RestockCartItems)
                .HasForeignKey(rci => rci.RestockCartId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<RestockCartItem>()
                .Property(r => r.Frequency)
                .HasConversion<string>();
            // Decimal precision for pricing

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.CartTotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.PriceAtPurchase)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            
            // Indexes (Performance + Safety)
            
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();
        }

        public DbSet<User> Users { get; set; } //creates Users table in the database
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CheckList> CheckLists { get; set; }
        public DbSet<CheckListItem> CheckListItems { get; set; }
        public DbSet<RestockCart> RestockCarts { get; set; }
        public DbSet<RestockCartItem> RestockCartItems { get; set; }
    }
}
