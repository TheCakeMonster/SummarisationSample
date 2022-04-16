
namespace SummarisationSample.ActivityService.Service.MessageHandling
{
    /// <summary>
    /// Contract to be supported by a message receiver
    /// </summary>
    /// <typeparam name="TKey">The data type of the key</typeparam>
    /// <typeparam name="TValue">The data type of the message being received</typeparam>
    internal interface IMessageReceiver<TKey, TValue>
    {
        event Func<TKey, TValue, Task>? OnMessageReceived;

        Task ReceiveMessages(string topics, CancellationToken stoppingToken);

        Task ConfirmMessageReceipt(TKey key);
    }
}