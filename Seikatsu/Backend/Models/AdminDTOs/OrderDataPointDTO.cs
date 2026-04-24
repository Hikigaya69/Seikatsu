namespace Seikatsu.Backend.Models.AdminDTOs
{
    public class OrderDataPointDTO
    {
        public string Label { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CreatedOrders { get; set; }
    }
}
