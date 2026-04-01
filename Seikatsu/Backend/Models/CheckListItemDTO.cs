namespace Seikatsu.Backend.Models
{
    public class CheckListItemDTO
    {
        public string ProductName { get; set; } = string.Empty;

        public bool IsChecked { get; set; }= false;
    }
}
