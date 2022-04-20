
namespace SummarisationSample.OrderService.Messaging
{
    public interface IMessagePublisher<TKey, TValue>
    {
        event Func<TKey, TValue, Task>? MessagePublished;
        event Func<TKey, TValue, Task>? MessagePublishingFailure;

        Task StartPublishingAsync(string topic, CancellationToken cancellationToken);
    }
}