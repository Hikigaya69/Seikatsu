using Seikatsu.Backend.Enums;

namespace Seikatsu.Backend.Models.AdminDTOs
{
    public class RevenuePeriodRequestDTO
    {
        public Guid? CategoryId { get; set; }
        public RevenuePeriod Period { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
