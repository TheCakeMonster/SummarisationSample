using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.OrderService.Messaging.Kafka
{
    public class KafkaPublicationConfiguration : IPublicationConfiguration
    {
        public string Topic { get; init; } = string.Empty;
    }
}
