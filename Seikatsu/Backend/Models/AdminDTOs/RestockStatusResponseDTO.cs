namespace Seikatsu.Backend.Models.AdminDTOs
{
    public class RestockStatusResponseDTO
    {
        public int activeRestockItemCount { get; set; }
        public int pausedRestockItemCount { get; set; }
        
        public int upcomingOrderCount { get; set; }
    }
}
