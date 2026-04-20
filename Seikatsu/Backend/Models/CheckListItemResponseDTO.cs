namespace Seikatsu.Backend.Models
{
    public class CheckListItemResponseDTO
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid? ProductId { get; set; } = null;
        public bool IsChecked { get; set; }
    }
}
