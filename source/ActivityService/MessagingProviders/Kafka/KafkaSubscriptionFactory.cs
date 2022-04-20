namespace SummarisationSample.ActivityService.Messaging.Kafka
{
    /// <summary>
    /// Subscription factory for providing Kafka subscription information by subscription name
    /// </summary>
    public class KafkaSubscriptionFactory : ISubscriptionFactory
    {
        /// <summary>
        /// Create an instance of subscription configuration for the named subscription
        /// </summary>
        /// <param name="subscriptionName">The name of the subscription for which information is required</param>
        /// <returns>An instance of KafkaSubscriptionConfiguration containing the subscription information</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ISubscriptionConfiguration GetSubscriptionConfiguration(string subscriptionName)
        {
            if (subscriptionName is null) throw new ArgumentNullException(subscriptionName);

            // Only one subsscription at the moment; extend if necessary
            return new KafkaSubscriptionConfiguration()
            {
                Topics = "summarisation.order",
                ConsumerGroupName = "activityservice"
            };
        }
    }
}
