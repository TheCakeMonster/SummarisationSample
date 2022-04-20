using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SummarisationSample.OrderService.Library;
using System.Collections.Concurrent;
using System.Net;

namespace SummarisationSample.OrderService.Messaging.Kafka
{
    /// <summary>
    /// Kafka implementation of a message publisher
    /// </summary>
    /// <typeparam name="TKey">The type of key in use</typeparam>
    /// <typeparam name="TValue">The type of message being published</typeparam>
    public class KafkaMessagePublisher<TKey, TValue> : IMessagePublisher<TKey, TValue>
    {
        private readonly string _busConnectionString;
        private readonly ILogger _logger;
        private readonly IMessageQueue<TKey, TValue> _messageQueue;
        private Task? _publishingTask;
        private int retryDelay = 50;

        public KafkaMessagePublisher(IMessageQueue<TKey, TValue> messageQueue, IConfiguration configuration, ILogger<KafkaMessagePublisher<TKey, TValue>> logger)
        {
            _messageQueue = messageQueue;
            _busConnectionString = configuration.GetConnectionString("MessageBus");
            _logger = logger;
        }

        public event Func<TKey, TValue, Task>? MessagePublished;

        public event Func<TKey, TValue, Task>? MessagePublishingFailure;

        /// <summary>
        /// Entrypoint for the long-running operation of publishing messages
        /// </summary>
        /// <param name="topic">The topic to which messages are being published</param>
        /// <param name="cancellationToken">The cancellation token used to manage execution</param>
        public Task StartPublishingAsync(string topic, CancellationToken cancellationToken)
        {
            _publishingTask = Task.Run(async () => await DoPublishingAsync(topic, cancellationToken));

            return Task.CompletedTask;
        }

        #region Private Helper Methods

        /// <summary>
        /// Start of background publishing; create a producer and then sit waiting for 
        /// items in the queue that can be published
        /// </summary>
        /// <param name="topic">The topic to be used for publishing</param>
        /// <param name="cancellationToken">The token used to manage the lifetime of the background service</param>
        private async Task DoPublishingAsync(string topic, CancellationToken cancellationToken)
        {
            using IProducer<TKey, TValue>? producer = await CreateProducerAsync(cancellationToken);
            if (producer is null) return;

            await PublishMessagesAsync(producer, topic, cancellationToken);
        }

        /// <summary>
        /// Create a producer of messages, with exception handling and basic retries
        /// </summary>
        /// <param name="cancellationToken">The token used to manage the lifetime of the operation</param>
        /// <returns>An instance of a producer that can produce messages</returns>
        private async Task<IProducer<TKey, TValue>?> CreateProducerAsync(CancellationToken cancellationToken)
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
                    await Task.Delay(1000, cancellationToken);
                }
            }

            return producer;
        }

        /// <summary>
        /// Long-running loop that checks for messages in the outbound queue and publishes them
        /// </summary>
        /// <param name="producer">The producer to use for publishing messages</param>
        /// <param name="topic">The topic to which to assign the messages</param>
        /// <param name="cancellationToken">The token used to manage the lifetime of the background operation</param>
        private async Task PublishMessagesAsync(IProducer<TKey, TValue> producer, string topic, CancellationToken cancellationToken)
        {
            QueueItem<TKey, TValue>? queueItem;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!_messageQueue.TryPeek(out queueItem))
                    {
                        await Task.Delay(25000, cancellationToken);
                        continue;
                    }

                    var result = await PublishMessageAsync(producer, topic, queueItem, cancellationToken);

                    _messageQueue.TryDequeue(out queueItem);
                }
                catch (OperationCanceledException)
                { }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while handling message!");
                }
            }

        }

        /// <summary>
        /// Attempt to publish a single message and inform consumers of the outcome
        /// </summary>
        /// <param name="producer">The producer through which to attempt to publish the message</param>
        /// <param name="topic">The topic to which to assign the message</param>
        /// <param name="queueItem">The item from the queue that is to be published</param>
        /// <param name="cancellationToken">The token used to manage the lifetime of the background operation</param>
        /// <returns>The delivery result from the producer</returns>
        private async Task<DeliveryResult<TKey, TValue>?> PublishMessageAsync(
            IProducer<TKey, TValue> producer, string topic, 
            QueueItem<TKey, TValue> queueItem, CancellationToken cancellationToken)
        {
            DeliveryResult<TKey, TValue>? result = null;
            Message<TKey, TValue> message;

            try
            {
                message = new Message<TKey, TValue>() { Key= queueItem.Key, Value = queueItem.Value };
                result = await producer.ProduceAsync(topic, message, cancellationToken);
                await OnPublishingSuccessAsync(queueItem);
                retryDelay = 50;
            }
            catch (OperationCanceledException)
            { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while publishing message!");
                await OnPublishingFailureAsync(queueItem);
                await Task.Delay(retryDelay, cancellationToken);
                if (retryDelay < 10000) retryDelay *= 2;
            }

            return result;
        }

        /// <summary>
        /// Raise the MessagePublished event to consumers, handling any exceptions they might throw
        /// </summary>
        /// <param name="message">The queue item that was published</param>
        private async Task OnPublishingSuccessAsync(QueueItem<TKey, TValue> message)
        {
            if (MessagePublished is not null)
            {
                try
                {
                    await MessagePublished.Invoke(message.Key, message.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in publishing success handler");
                }
            }
        }

        /// <summary>
        /// Raise the MessagePublishingFailure event to consumers, handling any exceptions they might throw
        /// </summary>
        /// <param name="message">The queue item that failed to be published</param>
        private async Task OnPublishingFailureAsync(QueueItem<TKey, TValue>? message)
        {
            if (message is not null && MessagePublishingFailure is not null)
            {
                try
                {
                    await MessagePublishingFailure.Invoke(message.Key, message.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in publishing failure handler");
                }
            }
        }

        #endregion

    }
}
