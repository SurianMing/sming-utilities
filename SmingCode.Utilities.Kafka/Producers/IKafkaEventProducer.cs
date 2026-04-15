namespace SmingCode.Utilities.Kafka.Producers;

public interface IKafkaProducer
{
    Task<bool> SendEvent<TValue>(
        string topic,
        TValue value
    );
    Task<bool> SendEvent<TKey, TValue>(
        string topic,
        TKey key,
        TValue value
    );
    Task<bool> SendEvent<TValue>(
        string topic,
        TValue value,
        Headers headers
    );
    Task<bool> SendEvent<TKey, TValue>(
        string topic,
        TKey key,
        TValue value,
        Headers headers
    );
}