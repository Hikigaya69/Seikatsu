namespace Seikatsu.Backend.Models.AdminDTOs
{
    public class RestockFrequncyCountResponseDTO
    {
        public int DailyCount { get; set; }
        public int WeeklyCount { get; set; }
        public int MonthlyCount { get; set; }
         public int BiWeeklyCount { get; set; }

        public int QuarterlyCount { get; set; }
    }
}
