namespace SmingCode.Utilities.Kafka.Producers;

public interface IKafkaProducerDelegateHandler<TKey, TValue>
{
    Task<bool> Next(
        IKafkaProducerContext<TKey, TValue> context
    );
}
