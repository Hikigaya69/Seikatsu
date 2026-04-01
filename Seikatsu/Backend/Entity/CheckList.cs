namespace Seikatsu.Backend.Entity
{
    public class CheckList
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<CheckListItem> Items { get; set; } = new();
    }
}
