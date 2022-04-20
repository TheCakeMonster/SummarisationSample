namespace SummarisationSample.ActivityService.Service.Contracts
{

    /// <summary>
    /// Utility extension methods for the DateOnly data type
    /// </summary>
    public static class DateOnlyExtensions
    {

        public static string ToDateOnlyString(this DateOnly date)
        {
            return date.ToString("yyyy-MM-dd");
        }

    }
}
