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
        }

        public DbSet<User> Users { get; set; } //creates Users table in the database
    }
}
