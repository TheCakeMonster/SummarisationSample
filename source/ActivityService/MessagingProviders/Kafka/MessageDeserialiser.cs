using Confluent.Kafka;
using System.Text.Json;

namespace SummarisationSample.ActivityService.Messaging.Kafka
{
    /// <summary>
    /// Simple JSON-based value deserialiser for Kafka
    /// </summary>
    /// <typeparam name="T">The type which is to be deserialised</typeparam>
    public class MessageDeserialiser<T> : IDeserializer<T>
    {
        /// <summary>
        /// Deserialise the data provided by Kafka into an instance of the required type
        /// </summary>
        /// <param name="data">The data being deserialised</param>
        /// <param name="isNull">Whether the data is null</param>
        /// <param name="context">The serialization coontext under which the operation is being performed</param>
        /// <returns>The deserialised type</returns>
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull) return default(T);
            return JsonSerializer.Deserialize<T>(data);
        }
    }
}
