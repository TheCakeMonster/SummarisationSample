namespace SummarisationSample.ActivityService.Messaging.Kafka
{
    public class KafkaSubscriptionConfiguration : ISubscriptionConfiguration
    {
        public string Topics { get; set; } = string.Empty;

        public string ConsumerGroupName { get; set; } = string.Empty;

    }
}
