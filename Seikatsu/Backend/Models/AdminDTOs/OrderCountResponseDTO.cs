namespace Seikatsu.Backend.Models.AdminDTOs
{
    public class OrderCountResponseDTO
    {
        public int TotalOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int CreatedOrders { get; set; }

        public int PendingOrders { get; set; }  

    }
}
