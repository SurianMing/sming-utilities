using System.Text;
using System.Text.Json;

namespace SmingCode.Utilities.Kafka.Producers;

internal class KafkaSerializerFactory
{
    internal static ISerializer<T> GetSerializer<T>()
        => typeof(T) == typeof(string)
            ? (ISerializer<T>)new KafkaStringSerializer()
            : new KafkaSerializer<T>();

    private class KafkaStringSerializer : ISerializer<string>
    {
        public byte[] Serialize(string value, SerializationContext context)
            => Encoding.UTF8.GetBytes(value);
    }

    private class KafkaSerializer<T> : ISerializer<T>
    {
        public byte[] Serialize(T value, SerializationContext context)
            => Encoding.UTF8.GetBytes(JsonSerializer.Serialize<T>(value));
    }
}
