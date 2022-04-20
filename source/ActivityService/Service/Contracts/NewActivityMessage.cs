namespace SummarisationSample.ActivityService.Service.Contracts
{
    /// <summary>
    /// Contract for the messages to be received and deserialised
    /// </summary>
    public class NewActivityMessage
    {

        public string MessageRef { get; set; } = string.Empty;

        public string ActivityTypeCode { get; set; } = string.Empty;

        public DateTime ActivityAt { get; set; } = DateTime.Now;
    }
}
