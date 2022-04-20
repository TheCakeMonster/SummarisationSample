namespace SummarisationSample.ActivityService.Messaging.Kafka
{
    public class KafkaSubscriptionFactory : ISubscriptionFactory
    {
        public ISubscriptionConfiguration GetSubscriptionConfiguration(string subscriptionName)
        {
            if (subscriptionName is null) throw new ArgumentNullException(subscriptionName);

            return new KafkaSubscriptionConfiguration()
            {
                Topics = "summarisation.order",
                ConsumerGroupName = "activityservice"
            };
        }
    }
}
