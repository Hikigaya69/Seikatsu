namespace Seikatsu.Backend.Models
{
    public class CheckListItemResponseDTO
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public bool IsChecked { get; set; }
    }
}
