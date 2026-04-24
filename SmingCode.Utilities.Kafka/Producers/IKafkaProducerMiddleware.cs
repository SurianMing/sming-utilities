namespace SmingCode.Utilities.Kafka.Producers;

public interface IKafkaProducerMiddleware
{
    Task<bool> HandleAsync<TKey, TValue>(
        IKafkaProducerContext<TKey, TValue> context,
        IKafkaProducerDelegateHandler<TKey, TValue> kafkaProducerDelegateHandler
    );
}

public delegate Task<bool> KafkaProducerDelegate<TKey, TValue>(
    IKafkaProducerContext<TKey, TValue> context
);
