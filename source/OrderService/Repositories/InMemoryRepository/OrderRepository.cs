using SummarisationSample.OrderService.Library;
using SummarisationSample.OrderService.Library.DataContracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.OrderService.Repositories.InMemoryRepository
{

    /// <summary>
    /// In-memory implementation of an Order repository
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private static int _maxOrderId = 0;
        private static ConcurrentBag<Order> _orders = new ConcurrentBag<Order>();
        private static ConcurrentBag<ActivityMessage> _messages = new ConcurrentBag<ActivityMessage>();
        private readonly IMessageQueue<string, ActivityMessage> _messageQueue;

        public OrderRepository(IMessageQueue<string, ActivityMessage> messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public Task<int> GetNextOrderId()
        {
            int nextOrderId = Interlocked.Increment(ref _maxOrderId);
            return Task.FromResult(nextOrderId);
        }

        public Task<Order?> GetOrderAsync(string orderRef)
        {
            Order? order;

            order = _orders.FirstOrDefault(o => o.OrderRef.Equals(orderRef, StringComparison.CurrentCultureIgnoreCase));
            return Task.FromResult(order);
        }

        public Task<IList<Order>> GetOrdersAsync()
        {
            IList<Order> orders;

            orders = _orders.ToList();
            return Task.FromResult(orders);
        }

        public Task<IList<Order>> GetOrdersForCustomerAsync(string customerRef)
        {
            IList<Order> orders;

            orders = _orders.Where(o => o.CustomerRef.Equals(customerRef, StringComparison.CurrentCultureIgnoreCase)).ToList();
            return Task.FromResult(orders);
        }

        public Task PlaceOrderAsync(Order order)
        {
            ActivityMessage activityMessage;

            activityMessage = new ActivityMessage()
            {
                ActivityTypeCode = ActivityTypeCodes.OrderCreated,
                ActivityAt = DateTime.Now
            };
            _orders.Add(order);
            _messages.Add(activityMessage);
            _messageQueue.Enqueue("order", activityMessage);

            return Task.CompletedTask;
        }

        public Task<IList<ActivityMessage>> GetUnpublishedActivityMessagesAsync()
        {
            IList<ActivityMessage> messages;

            // Read unpublished messages, excluding any that are considered poison messages
            messages = _messages.Where(
                m => !m.PublishedAt.HasValue && m.PublishingFailures < 10)
                .OrderBy(m => m.ActivityAt)
                .ToList();
            return Task.FromResult(messages);
        }

        public Task MarkActivityMessagePublishedAsync(string messageRef)
        {
            ActivityMessage? message;

            message = _messages.FirstOrDefault(m => m.MessageRef.Equals(messageRef));
            if (message is null) throw new ArgumentOutOfRangeException(nameof(messageRef));

            message.PublishedAt = DateTime.Now;

            return Task.CompletedTask;
        }

        public Task RecordActivityMessagePublishingFailureAsync(string messageRef)
        {
            ActivityMessage? message;

            message = _messages.FirstOrDefault(m => m.MessageRef.Equals(messageRef));
            if (message is null) throw new ArgumentOutOfRangeException(nameof(messageRef));

            message.PublishedAt = DateTime.Now;

            return Task.CompletedTask;
        }

        public Task<bool> SaveChangesAsync()
        {
            return Task.FromResult(true);
        }
    }
}
