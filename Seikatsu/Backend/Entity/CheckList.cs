namespace Seikatsu.Backend.Entity
{
    public class CheckList
    {
        public Guid Id { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public bool IsChecked { get; set; }

        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }
}
