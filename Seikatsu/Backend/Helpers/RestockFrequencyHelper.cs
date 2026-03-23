using Seikatsu.Backend.Enums;

namespace Seikatsu.Backend.Helpers
{
    public class RestockFrequencyHelper
    {
        public static DateTime ComputeNextOrderDate(
            RestockFrequency frequency,
            DateTime from)
        {
            return frequency switch
            {
                RestockFrequency.Daily => from.AddDays(1),
                RestockFrequency.Weekly => from.AddDays(7),
                RestockFrequency.BiWeekly => from.AddDays(14),
                RestockFrequency.Monthly => from.AddMonths(1),
                RestockFrequency.Quarterly => from.AddMonths(3),
                _ => throw new ArgumentOutOfRangeException(nameof(frequency), "Invalid frequency.")
            };
        }
    }
}
