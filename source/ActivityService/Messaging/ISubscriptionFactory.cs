namespace SummarisationSample.ActivityService.Messaging
{
    public interface ISubscriptionFactory
    {
        ISubscriptionConfiguration GetSubscriptionConfiguration(string subscriptionName);
    }
}
