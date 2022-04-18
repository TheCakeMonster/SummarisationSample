using Confluent.Kafka;
using System.Collections.Concurrent;
using System.Net;

namespace SummarisationSample.OrderService.Service.Messaging
{
    public class MessagePublisher<TKey, TValue>
    {
        private readonly string _busConnectionString;
        private readonly ILogger _logger;
        private readonly ConcurrentQueue<Message<TKey, TValue>> _messages = new ConcurrentQueue<Message<TKey, TValue>>();

        public MessagePublisher(IConfiguration configuration, ILogger<MessagePublisher<TKey, TValue>> logger)
        {
            _busConnectionString = configuration.GetConnectionString("MessageBus");
            _logger = logger;
        }

        public void EnqueueMessage(TKey messageKey, TValue messageValue)
        {
            _messages.Enqueue(new Message<TKey, TValue> { Key = messageKey, Value = messageValue });
        }

        public async Task PerformPublishingAsync(CancellationToken cancellationToken, string topic)
        {

            using IProducer<TKey, TValue>? producer = await CreateProducerAsync(cancellationToken);
            if (producer is null) return;

            await PublishMessagesAsync(cancellationToken, producer, topic);
        }

        #region Private Helper Methods

        private async Task<IProducer<TKey, TValue>>? CreateProducerAsync(CancellationToken cancellationToken)
        {
            IProducer<TKey, TValue>? producer = null;

            var config = new ProducerConfig
            {
                BootstrapServers = _busConnectionString,
                ClientId = Dns.GetHostName()
            };

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    producer = new ProducerBuilder<TKey, TValue>(config)
                        .SetValueSerializer(new MessageSerialiser<TValue>())
                        .Build();
                    break;
                }
                catch (OperationCanceledException)
                { }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to connect to publisher!");
                    await Task.Delay(1000);
                }
            }

            return producer;
        }

        private async Task PublishMessagesAsync(CancellationToken cancellationToken, IProducer<TKey, TValue> producer, string topic)
        {
            Message<TKey, TValue>? message;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!_messages.TryPeek(out message))
                    {
                        await Task.Delay(100);
                        continue;
                    }

                    var result = await producer.ProduceAsync(topic, message);

                    _messages.TryDequeue(out message);
                }
                catch (OperationCanceledException)
                { }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while publishing message!");
                    await Task.Delay(50);
                }
            }

        }

        #endregion

    }
}
