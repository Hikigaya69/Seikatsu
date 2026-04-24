namespace Seikatsu.Backend.Models.AdminDTOs
{
    public class RevenueResponseDTO
    {
        public decimal TotalRevenue { get; set; }
        public IEnumerable<RevenueDatePointResponseDTO> DataPoints { get; set; } = Enumerable.Empty<RevenueDatePointResponseDTO>();
    }
}
