namespace Seikatsu.Backend.Models
{
    public class CheckListItemDTO
    {
        public string ProductName { get; set; } = string.Empty;

        public Guid? ProductId { get; set; } = null;
        public bool IsChecked { get; set; }= false;
    }
}
