using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SummarisationSample.ActivityService.Messaging.Kafka
{

    /// <summary>
    /// Kafka implementation of a message receiver
    /// </summary>
    /// <typeparam name="TKey">The type of key in use</typeparam>
    /// <typeparam name="TValue">The type of message being received</typeparam>
    internal class KafkaMessageReceiver<TKey, TValue> : IMessageReceiver<TKey, TValue>
    {
        private readonly ConsumerConfig _busConfiguration;
        private readonly ISubscriptionFactory _subscriptionFactory;
        private readonly ILogger _logger;
        private Task? _receiptTask;

        public event Func<TKey, TValue, Task>? OnMessageReceived;

        public KafkaMessageReceiver(IConfiguration configuration, ISubscriptionFactory subscriptionFactory, ILogger<KafkaMessageReceiver<TKey, TValue>> logger)
        {
            string connectionString = configuration.GetConnectionString("MessageBus");
            _busConfiguration = new ConsumerConfig()
            {
                BootstrapServers = connectionString,
                EnableAutoCommit = true,
                EnableAutoOffsetStore = false
            };
            _subscriptionFactory = subscriptionFactory;
            _logger = logger;
        }

        /// <summary>
        /// Entry point for long-running message receipt
        /// </summary>
        /// <param name="subscriptionName">The name of the subscription to be used by the receiver</param>
        /// <param name="cancellationToken">Cancellation token used to manage shutdown</param>
        public Task StartMessageReceiptAsync(string subscriptionName, CancellationToken cancellationToken)
        {
            ISubscriptionConfiguration untypedConfig;
            KafkaSubscriptionConfiguration? subscriptionConfig;

            untypedConfig = _subscriptionFactory.GetSubscriptionConfiguration(subscriptionName);
            if (untypedConfig is null) throw new ArgumentException(nameof(subscriptionName));
            subscriptionConfig = untypedConfig as KafkaSubscriptionConfiguration;
            if (subscriptionConfig is null) throw new ArgumentException(nameof(untypedConfig));

            _busConfiguration.GroupId = subscriptionConfig.ConsumerGroupName;
            _receiptTask = Task.Run(async () => await ReceiveMessagesAsync(subscriptionConfig.Topics, cancellationToken));

            return Task.CompletedTask;
        }

        private async Task ReceiveMessagesAsync(string topics, CancellationToken cancellationToken)
        {
            using IConsumer<TKey, TValue>? consumer = await CreateConsumer(topics, cancellationToken);
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

        private async Task<IConsumer<TKey, TValue>?> CreateConsumer(string topics, CancellationToken cancellationToken)
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
                    _logger.LogError(ex, "Exception during connection to Kafka!");
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
