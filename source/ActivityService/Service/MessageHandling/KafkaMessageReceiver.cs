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
        private readonly ConsumerConfig _busConfiguration;
        private readonly ILogger _logger;

        public event Func<TKey, TValue, Task>? OnMessageReceived;

        public KafkaMessageReceiver(IConfiguration configuration, ILogger<KafkaMessageReceiver<TKey, TValue>> logger)
        {
            string connectionString = configuration.GetConnectionString("MessageBus");
            _busConfiguration = new ConsumerConfig()
            {
                BootstrapServers = connectionString,
                GroupId = "activityservice",
                EnableAutoCommit = true,
                EnableAutoOffsetStore = false
            };
            _logger = logger;
        }

        /// <summary>
        /// Entry point for long-running message receipt
        /// </summary>
        /// <param name="cancellationToken">Cancellation token used to manage shutdown</param>
        /// <param name="topics">The topics to which to subscribe</param>
        public async Task ReceiveMessages(CancellationToken cancellationToken, string topics)
        {
            using var consumer = await CreateConsumer(cancellationToken, topics);
            if (consumer is null) return;

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

            if (consumer is not null) consumer.Close();

        }

        private async Task<IConsumer<TKey, TValue>?> CreateConsumer(CancellationToken cancellationToken, string topics)
        {
            IConsumer<TKey, TValue>? consumer = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    consumer = new ConsumerBuilder<TKey, TValue>(_busConfiguration)
                        .SetValueDeserializer(new MessageDeserialiser<TValue>())
                        .Build();

                    consumer.Subscribe(topics);
                    break;
                }
                catch (OperationCanceledException)
                { }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception during connection to bus!");
                    await Task.Delay(5000);
                }
            }

            return consumer;
        }

        /// <summary>
        /// Await a single message and then deal with it
        /// </summary>
        /// <param name="consumer">The client from which messages are to be received</param>
        /// <param name="cancellationToken">Cancellation token used for handling graceful shutdown</param>
        private async Task AwaitMessageReceipt(IConsumer<TKey, TValue> consumer, CancellationToken cancellationToken)
        {
            ConsumeResult<TKey, TValue> consumeResult = consumer.Consume(cancellationToken);

            if (OnMessageReceived is not null)
            {
                await OnMessageReceived.Invoke(consumeResult.Message.Key, consumeResult.Message.Value);
            }

            consumer.StoreOffset(consumeResult);
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
