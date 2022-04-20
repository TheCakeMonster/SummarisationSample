using Confluent.Kafka;
using System.Text;
using System.Text.Json;

namespace SummarisationSample.OrderService.Service.Messaging
{
    public class MessageSerialiser<TValue> : ISerializer<TValue>
    {
        public byte[] Serialize(TValue data, SerializationContext context)
        {
            string message;

            message = JsonSerializer.Serialize(data);
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
