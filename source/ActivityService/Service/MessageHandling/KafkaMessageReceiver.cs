using Confluent.Kafka;

namespace SummarisationSample.ActivityService.Service.MessageHandling
{

    /// <summary>
    /// Kafka implementation of a message receiver
    /// </summary>
    /// <typeparam name="TKey">The type of key in use</typeparam>
    /// <typeparam name="TValue">The type of message being received</typeparam>
    internal class KafkaMessageReceiver<TKey, TValue> : IMessageReceiver<TKey, TValue>
    {
        private readonly ConsumerConfig _configuration;
        private readonly ILogger _logger;

        public event Func<TKey, TValue, Task>? OnMessageReceived;

        public KafkaMessageReceiver(ClientConfig options, ILogger<KafkaMessageReceiver<TKey, TValue>> logger)
        {
            _configuration = new ConsumerConfig(options);
            _logger = logger;
        }

        /// <summary>
        /// Entry point for long-running message receipt
        /// </summary>
        /// <param name="topics">The topics to which to subscribe</param>
        /// <param name="cancellationToken">Cancellation token used to manage shutdown</param>
        public async Task ReceiveMessages(string topics, CancellationToken cancellationToken)
        {
            var consumer = new ConsumerBuilder<TKey, TValue>(_configuration)
                .Build();

            consumer.Subscribe(topics);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await AwaitMessageReceipt(consumer, cancellationToken);
                }
                catch (OperationCanceledException)
                { }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception during message receipt");
                }
            }

            consumer.Close();
            consumer.Dispose();
        }

        /// <summary>
        /// Await a single message and then deal with it
        /// </summary>
        /// <param name="consumer">The client from which messages are to be received</param>
        /// <param name="cancellationToken">Cancellation token used for handling graceful shutdown</param>
        private async Task AwaitMessageReceipt(IConsumer<TKey, TValue> consumer, CancellationToken cancellationToken)
        {
            ConsumeResult<TKey, TValue> consumed = consumer.Consume(cancellationToken);

            if (OnMessageReceived is not null)
            {
                await OnMessageReceived.Invoke(consumed.Message.Key, consumed.Message.Value);
            }

            consumer.Commit();
        }

        /// <summary>
        /// Confirm receipt of a message, if necessary
        /// </summary>
        /// <param name="key">The key of the message we are to confirm</param>
        public Task ConfirmMessageReceipt(TKey key)
        {
            return Task.CompletedTask;
        }
    }
}
