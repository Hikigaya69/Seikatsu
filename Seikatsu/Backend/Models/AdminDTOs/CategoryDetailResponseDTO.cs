namespace Seikatsu.Backend.Models.AdminDTOs
{
    public class CategoryDetailResponseDTO
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public int ProductCount { get; set; }
    }
}
