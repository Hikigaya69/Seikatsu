namespace Seikatsu.Backend.Entity
{
    public class Category
    {
        public Guid Id { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
