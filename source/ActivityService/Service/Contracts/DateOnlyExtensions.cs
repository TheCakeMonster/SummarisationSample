namespace SummarisationSample.ActivityService.Service.Contracts
{
    public static class DateOnlyExtensions
    {

        public static string ToDateOnlyString(this DateOnly date)
        {
            return date.ToString("yyyy-MM-dd");
        }

    }
}
