using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.OrderService.Library
{
    public class QueueItem<TKey, TValue>
    {

        public TKey Key { get; init; }

        public TValue Value { get; init; }

    }
}
