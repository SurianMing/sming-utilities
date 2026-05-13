namespace SmingCode.Utilities.Kafka.Consumers;

public class KafkaConsumerContext
{
    internal KafkaConsumerContext(
        string topicConsumed,
        Headers headers,
        object? key,
        Type keyType,
        object? value,
        Type valueType,
        Func<KafkaConsumerContext, Task<KafkaEventResult>> messageConsumer,
        IServiceProvider serviceProvider
    ) => (TopicConsumed, Headers, Key, KeyType, Value, ValueType, MessageConsumer, ServiceProvider)
            = (topicConsumed, headers, key, keyType, value, valueType, messageConsumer, serviceProvider);

    internal IServiceProvider ServiceProvider { get; }
    internal Func<KafkaConsumerContext, Task<KafkaEventResult>> MessageConsumer { get; }

    public string TopicConsumed { get; }
    public Headers Headers { get; }
    public object? Key { get; }
    public Type KeyType { get; }
    public object? Value { get; }
    public Type ValueType { get; }
}
