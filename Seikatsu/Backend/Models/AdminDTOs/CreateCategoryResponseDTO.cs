namespace Seikatsu.Backend.Models.AdminDTOs
{
    public class CreateCategoryResponseDTO
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
