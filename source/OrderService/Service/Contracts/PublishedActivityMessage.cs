namespace SummarisationSample.OrderService.Service.Contracts
{
    internal class PublishedActivityMessage
    {
        public string MessageRef { get; set; } = string.Empty;

        public string ActivityTypeCode { get; set; } = string.Empty;

        public DateTime ActivityAt { get; set; } = DateTime.Now;

    }
}