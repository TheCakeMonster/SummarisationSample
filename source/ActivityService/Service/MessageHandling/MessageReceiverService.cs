using Confluent.Kafka;
using SummarisationSample.ActivityService.Library;
using SummarisationSample.ActivityService.Library.DataContracts;
using SummarisationSample.ActivityService.Service.Contracts;

namespace SummarisationSample.ActivityService.Service.MessageHandling
{

    /// <summary>
    /// Background service that is responsible for handling messages
    /// </summary>
    internal class MessageReceiverService : BackgroundService
    {
        private readonly IMessageReceiver<string, NewActivityMessage> _messageReceiver;
        private readonly IServiceScope _scope;

        public MessageReceiverService(IServiceScopeFactory scopeFactory)
        {
            _scope = scopeFactory.CreateScope();
            _messageReceiver = _scope.ServiceProvider.GetRequiredService<IMessageReceiver<string, NewActivityMessage>>();
            _messageReceiver.OnMessageReceived += MessageReceiver_OnMessageReceived;
        }

        /// <summary>
        /// Long-running operation
        /// </summary>
        /// <param name="stoppingToken">Cancellation token, used by the host to request shutdown</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            await _messageReceiver.ReceiveMessages(stoppingToken, "summarisation.order");
        }

        /// <summary>
        /// Handling of the shutdown request from the host
        /// </summary>
        /// <param name="cancellationToken">The cancellation token provided by the host</param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _scope.Dispose();
            return base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Handler for receipt of a message from the receiver in use
        /// </summary>
        /// <param name="key">The message key</param>
        /// <param name="message">The received message</param>
        private async Task MessageReceiver_OnMessageReceived(string key, NewActivityMessage message)
        {
            IActivityMessage activityMessage;
            IActivityRepository repository = _scope.ServiceProvider.GetRequiredService<IActivityRepository>();

            activityMessage = message.ToActivityMessage();
            await repository.RecordActivity(activityMessage);

            await _messageReceiver.ConfirmMessageReceipt(key);
        }

    }
}
