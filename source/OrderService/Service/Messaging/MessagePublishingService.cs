using SummarisationSample.OrderService.Library;
using SummarisationSample.OrderService.Library.DataContracts;
using SummarisationSample.OrderService.Service.Contracts;

namespace SummarisationSample.OrderService.Service.Messaging
{
    public class MessagePublishingService : BackgroundService
    {
        private readonly IServiceScope _scope;
        private readonly ILogger _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageQueue<string, ActivityMessage> _sourceQueue;
        private readonly IMessageQueue<string, PublishedActivityMessage> _publishingQueue;

        public MessagePublishingService(IServiceScopeFactory serviceScopeFactory)
        {
            _scope = serviceScopeFactory.CreateScope();
            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<MessagePublishingService>>();
            _orderRepository = _scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            _sourceQueue = _scope.ServiceProvider.GetRequiredService<IMessageQueue<string, ActivityMessage>>();
            _sourceQueue.Enqueued += Source_OnEnqueued;
            _publishingQueue = _scope.ServiceProvider.GetRequiredService<IMessageQueue<string, PublishedActivityMessage>>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IMessagePublisher<string, PublishedActivityMessage> publisher;

            publisher = _scope.ServiceProvider.GetRequiredService<IMessagePublisher<string, PublishedActivityMessage>>();
            publisher.MessagePublished += Publisher_OnMessagePublished;
            publisher.MessagePublishingFailure += Publisher_OnMessagePublishingFailure;
            await EnqueueUnpublishedMessages(_publishingQueue, _orderRepository);

            await publisher.PerformPublishingAsync(stoppingToken, "summarisation.order");
        }

        #region Private Helper Methods

        /// <summary>
        /// Load all of the previously unpublished messages from the data store and enqueue them
        /// </summary>
        /// <param name="messagePublisher">The publisher of messages</param>
        /// <param name="orderRepository">The repository from which unpublished messages can be retrieved</param>
        /// <returns></returns>
        private async Task EnqueueUnpublishedMessages(IMessageQueue<string, PublishedActivityMessage> messagePublisher, IOrderRepository orderRepository)
        {
            IList<Library.ActivityMessage> unpublishedMessages;

            unpublishedMessages = await orderRepository.GetUnpublishedActivityMessagesAsync();
            foreach (Library.ActivityMessage unpublishedMessage in unpublishedMessages)
            {
                messagePublisher.Enqueue("order", unpublishedMessage.ToPublishedActivityMessage());
            }
        }

        private void Source_OnEnqueued()
        {
            QueueItem<string, ActivityMessage>? queueItem;

            while (_sourceQueue.TryDequeue(out queueItem))
            {
                if (queueItem is null) continue;
                _publishingQueue.Enqueue(queueItem.Key, queueItem.Value.ToPublishedActivityMessage());
            }
        }

        private async Task Publisher_OnMessagePublished(string key, PublishedActivityMessage message)
        {
            await _orderRepository.MarkActivityMessagePublishedAsync(message.MessageRef);
        }

        private async Task Publisher_OnMessagePublishingFailure(string key, PublishedActivityMessage message)
        {
            await _orderRepository.RecordActivityMessagePublishingFailureAsync(message.MessageRef);
        }

        #endregion
    }
}
