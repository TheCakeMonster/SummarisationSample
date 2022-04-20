using SummarisationSample.OrderService.Library;
using SummarisationSample.OrderService.Library.DataContracts;
using SummarisationSample.OrderService.Messaging;
using SummarisationSample.OrderService.Service.Contracts;

namespace SummarisationSample.OrderService.Service.Messaging
{
    /// <summary>
    /// Background service that is responsible for publishing messages
    /// </summary>
    public class MessagePublishingService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;
        private readonly IMessageQueue<string, ActivityMessage> _sourceQueue;
        private readonly IMessageQueue<string, PublishedActivityMessage> _publishingQueue;
        private readonly IMessagePublisher<string, PublishedActivityMessage> _publisher;

        public MessagePublishingService(IServiceScopeFactory serviceScopeFactory, 
            IMessageQueue<string, ActivityMessage> sourceQueue, 
            IMessageQueue<string, PublishedActivityMessage> publishingQueue,
            IMessagePublisher<string, PublishedActivityMessage> publisher,
            ILogger<MessagePublishingService> logger)
        {
            _scopeFactory = serviceScopeFactory;
            _logger = logger;
            _sourceQueue = sourceQueue;
            _sourceQueue.Enqueued += Source_OnEnqueued;
            _publishingQueue = publishingQueue;
            _publisher = publisher;
            _publisher.MessagePublished += Publisher_OnMessagePublished;
            _publisher.MessagePublishingFailure += Publisher_OnMessagePublishingFailure;
        }

        /// <summary>
        /// Entry point of the background service, which initiates the work to be done
        /// </summary>
        /// <param name="stoppingToken">The token used to control the lifetime of the background service</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await EnqueueUnpublishedMessagesAsync(_publishingQueue);

            await _publisher.StartPublishingAsync("Order", stoppingToken);
        }

        #region Private Helper Methods

        /// <summary>
        /// Load all of the previously unpublished messages from the data store and enqueue them
        /// </summary>
        /// <param name="messagePublisher">The publisher of messages</param>
        /// <param name="orderRepository">The repository from which unpublished messages can be retrieved</param>
        /// <returns></returns>
        private async Task EnqueueUnpublishedMessagesAsync(IMessageQueue<string, PublishedActivityMessage> messagePublisher)
        {
            IList<Library.ActivityMessage> unpublishedMessages;
            IOrderRepository orderRepository;

            using IServiceScope serviceScope = _scopeFactory.CreateScope();
            orderRepository = serviceScope.ServiceProvider.GetRequiredService<IOrderRepository>();

            unpublishedMessages = await orderRepository.GetUnpublishedActivityMessagesAsync();
            foreach (Library.ActivityMessage unpublishedMessage in unpublishedMessages)
            {
                messagePublisher.Enqueue("order", unpublishedMessage.ToPublishedActivityMessage());
            }
        }

        /// <summary>
        /// Handler for the Enqueued event in the source message queue
        /// Translates items from the source and puts them in the publishing queue
        /// </summary>
        private void Source_OnEnqueued()
        {
            QueueItem<string, ActivityMessage>? queueItem;

            while (_sourceQueue.TryDequeue(out queueItem))
            {
                if (queueItem is null) break;
                _publishingQueue.Enqueue(queueItem.Key, queueItem.Value.ToPublishedActivityMessage());
            }
        }

        /// <summary>
        /// Handler for the publisher's MessagePublished event, using the repository to 
        /// mark an activity message as having been published
        /// </summary>
        /// <param name="key">The key of the message published</param>
        /// <param name="message">The message that was published</param>
        private async Task Publisher_OnMessagePublished(string key, PublishedActivityMessage message)
        {
            IOrderRepository orderRepository;

            using IServiceScope serviceScope = _scopeFactory.CreateScope();
            orderRepository = serviceScope.ServiceProvider.GetRequiredService<IOrderRepository>();
            await orderRepository.MarkActivityMessagePublishedAsync(message.MessageRef);
        }

        /// <summary>
        /// Handler for the publisher's MessagePublishingFailure event, using the repository to 
        /// mark an activity message as having failed publishing (to control poison messages)
        /// </summary>
        /// <param name="key">The key of the message that failed to publish</param>
        /// <param name="message">The message that failed to publish</param>
        private async Task Publisher_OnMessagePublishingFailure(string key, PublishedActivityMessage message)
        {
            IOrderRepository orderRepository;

            using IServiceScope serviceScope = _scopeFactory.CreateScope();
            orderRepository = serviceScope.ServiceProvider.GetRequiredService<IOrderRepository>();
            await orderRepository.RecordActivityMessagePublishingFailureAsync(message.MessageRef);
        }

        #endregion
    }
}
