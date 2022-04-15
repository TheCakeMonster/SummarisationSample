namespace SummarisationSample.ActivityService.Service.Contracts
{
    public class ActivitySummary
    {
        public DateOnly ActivityDate { get; set; }

        public string ActivityTypeCode { get; set; }

        public string ActivityTypeName { get; set; }

        public int Quantity { get; set; }
    }
}
