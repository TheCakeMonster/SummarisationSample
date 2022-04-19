namespace SummarisationSample.ActivityService.Service.Contracts
{
    public class ActivitySummary
    {
        public string ActivityDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        public string ActivityTypeCode { get; set; } = string.Empty;

        public string ActivityTypeName { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}
