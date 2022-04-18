namespace SummarisationSample.OrderService.Service.Messaging
{
    public class MessagePublishingService : BackgroundService
    {
        private readonly IServiceScope _scope;
        private readonly ILogger _logger;

        public MessagePublishingService(IServiceScopeFactory serviceScopeFactory)
        {
            _scope = serviceScopeFactory.CreateScope();
            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<MessagePublishingService>>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            MessagePublisher<string, ActivityMessage> publisher;

            publisher = _scope.ServiceProvider.GetRequiredService<MessagePublisher<string, ActivityMessage>>();
            EnqueueTestMessage(publisher);

            await publisher.PerformPublishingAsync(stoppingToken, "summarisation.order");
        }

        #region Private Helper Methods

        private void EnqueueTestMessage(MessagePublisher<string, ActivityMessage> publisher)
        {
            ActivityMessage activityMessage;

            try
            {
                activityMessage = new ActivityMessage()
                {
                    ActivityTypeCode = "ORDER_CREATED"
                };

                publisher.EnqueueMessage("order", activityMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error whilst enqueuing test message!");
            }
        }

        #endregion
    }
}
