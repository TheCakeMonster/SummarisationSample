namespace SummarisationSample.OrderService.Contracts
{
    public class OrderSummary
    {
        public string OrderRef { get; set; } = string.Empty;

        public string CustomerRef { get; set; } = string.Empty;

        public int NumberOfItems { get; set; } = 0;

        public string StatusName { get; set; } = "Unknown";

    }
}
