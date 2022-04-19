
using SummarisationSample.OrderService.Library;

namespace SummarisationSample.OrderService.Service.Messaging
{
    public interface IMessagePublisher<TKey, TValue>
    {
        event Func<TKey, TValue, Task>? MessagePublished;
        event Func<TKey, TValue, Task>? MessagePublishingFailure;

        Task PerformPublishingAsync(CancellationToken cancellationToken, string topic);
    }
}