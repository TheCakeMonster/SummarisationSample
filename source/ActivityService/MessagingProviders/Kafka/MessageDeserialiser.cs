﻿using Confluent.Kafka;
using System.Text.Json;

namespace SummarisationSample.ActivityService.Messaging.Kafka
{
    public class MessageDeserialiser<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<T>(data);
        }
    }
}
