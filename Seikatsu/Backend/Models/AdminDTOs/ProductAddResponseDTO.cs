namespace Seikatsu.Backend.Models.AdminDTOs
{
    public class ProductAddResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public decimal Price { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
