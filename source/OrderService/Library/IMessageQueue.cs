using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.OrderService.Library
{
    public interface IMessageQueue<TKey, TValue>
    {
        event Action Enqueued;

        void Enqueue(TKey key, TValue message);

        bool TryPeek(out QueueItem<TKey, TValue>? queueItem);

        bool TryDequeue(out QueueItem<TKey, TValue>? queueItem);

    }
}
