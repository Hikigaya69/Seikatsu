namespace Seikatsu.Backend.Models.AdminDTOs
{
    public class OrderAnalysisResponseDTO
    {
        public int TotalOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CreatedOrders { get; set; }
        public IEnumerable<OrderDataPointDTO> DataPoints { get; set; } = Enumerable.Empty<OrderDataPointDTO>();
    }
}
