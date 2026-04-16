using System.Text;
using System.Text.Json;

namespace SmingCode.Utilities.Kafka.Host.Consumers;

internal class KafkaDeserializerFactory
{
    internal static IDeserializer<T> GetDeserializer<T>()
        => typeof(T) == typeof(string)
            ? (IDeserializer<T>)new KafkaStringDeserializer()
            : new KafkaDeserializer<T>();

    private class KafkaStringDeserializer : IDeserializer<string>
    {
        public string Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            => Encoding.UTF8.GetString(data);
    }

    private class KafkaDeserializer<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            => !isNull
                ? JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data))!
                : throw new InvalidCastException("Attempt to do stuff you just can't");
    }
}
