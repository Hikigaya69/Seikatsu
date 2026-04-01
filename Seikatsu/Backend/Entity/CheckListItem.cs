namespace Seikatsu.Backend.Entity
{
    public class CheckListItem
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public bool IsChecked { get; set; }
        public Guid CheckListId { get; set; }
        public CheckList? CheckList { get; set; }
    }
}
