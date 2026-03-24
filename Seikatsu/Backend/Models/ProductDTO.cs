namespace Seikatsu.Backend.Models
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsFood { get; set; }
        public string StorageType { get; set; } = string.Empty;

        public string ProductImageUrl { get; set; } = string.Empty;

        public string CountryName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
