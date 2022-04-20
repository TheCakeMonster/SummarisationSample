namespace SummarisationSample.ActivityService.Messaging.Kafka
{
    /// <summary>
    /// Type used to transport subscription information to a Kafka consumer
    /// </summary>
    public class KafkaSubscriptionConfiguration : ISubscriptionConfiguration
    {
        public string Topics { get; set; } = string.Empty;

        public string ConsumerGroupName { get; set; } = string.Empty;

    }
}
