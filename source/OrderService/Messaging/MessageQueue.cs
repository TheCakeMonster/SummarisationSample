using SummarisationSample.OrderService.Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.OrderService.Messaging
{
    public class MessageQueue<TKey, TValue> : IMessageQueue<TKey, TValue>
    {
        private readonly ConcurrentQueue<QueueItem<TKey, TValue>> _messageQueue = new ConcurrentQueue<QueueItem<TKey, TValue>>();

        public event Action? Enqueued;

        public void Enqueue(TKey key, TValue message)
        {
            _messageQueue.Enqueue(new QueueItem<TKey, TValue> { Key = key, Value = message });
            if (Enqueued is not null) Enqueued.Invoke();
        }

        public bool TryDequeue(out QueueItem<TKey, TValue>? queueItem)
        {
            return _messageQueue.TryDequeue(out queueItem);
        }

        public bool TryPeek(out QueueItem<TKey, TValue>? queueItem)
        {
            return _messageQueue.TryPeek(out queueItem);
        }
    }
}
