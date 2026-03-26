namespace Seikatsu.Backend.Models
{
    public class OrderOverviewDTO
    {
        public decimal TotalAmountSpent { get; set; }
        public int TotalOrders { get; set; }
        public  int DeliveredOrders { get; set; }
         public int PendingOrders { get; set; }

        public int CancelledOrders { get; set; }
    }
}
