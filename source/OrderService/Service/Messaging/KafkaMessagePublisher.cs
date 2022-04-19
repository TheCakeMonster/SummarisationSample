﻿using Confluent.Kafka;
using SummarisationSample.OrderService.Library;
using SummarisationSample.OrderService.Library.DataContracts;
using System.Collections.Concurrent;
using System.Net;

namespace SummarisationSample.OrderService.Service.Messaging
{
    public class KafkaMessagePublisher<TKey, TValue> : IMessagePublisher<TKey, TValue>
    {
        private readonly string _busConnectionString;
        private readonly ILogger _logger;
        private readonly IMessageQueue<TKey, TValue> _queue;
        private int retryDelay = 50;

        public KafkaMessagePublisher(IMessageQueue<TKey, TValue> messageQueue, IConfiguration configuration, ILogger<KafkaMessagePublisher<TKey, TValue>> logger)
        {
            _queue = messageQueue;
            _busConnectionString = configuration.GetConnectionString("MessageBus");
            _logger = logger;
        }

        public event Func<TKey, TValue, Task>? MessagePublished;

        public event Func<TKey, TValue, Task>? MessagePublishingFailure;

        public async Task PerformPublishingAsync(CancellationToken cancellationToken, string topic)
        {

            using IProducer<TKey, TValue>? producer = await CreateProducerAsync(cancellationToken);
            if (producer is null) return;

            await PublishMessagesAsync(producer, topic, cancellationToken);
        }

        #region Private Helper Methods

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
                    await Task.Delay(1000);
                }
            }

            return producer;
        }

        private async Task PublishMessagesAsync(IProducer<TKey, TValue> producer, string topic, CancellationToken cancellationToken)
        {
            QueueItem<TKey, TValue>? queueItem;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!_queue.TryPeek(out queueItem))
                    {
                        await Task.Delay(25);
                        continue;
                    }

                    var result = await PublishMessageAsync(producer, topic, queueItem, cancellationToken);

                    _queue.TryDequeue(out queueItem);
                }
                catch (OperationCanceledException)
                { }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while handling message!");
                }
            }

        }

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
                await Task.Delay(retryDelay);
                if (retryDelay < 10000) retryDelay *= 2;
            }

            return result;
        }

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