namespace SummarisationSample.ActivityService.Service.Contracts
{
    public class NewActivityMessage
    {

        public string MessageRef { get; set; } = MessageRefGenerator.Generate();

        public string ActivityTypeCode { get; set; } = string.Empty;

        public DateTime ActivityAt { get; set; } = DateTime.Now;
    }
}
