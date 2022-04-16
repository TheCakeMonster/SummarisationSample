namespace SummarisationSample.ActivityService.Service.Contracts
{
    public class ActivitySummary
    {
        public DateOnly ActivityDate { get; set; }

        public string ActivityTypeCode { get; set; } = string.Empty;

        public string ActivityTypeName { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}
