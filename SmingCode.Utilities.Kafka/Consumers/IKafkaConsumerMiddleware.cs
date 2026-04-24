namespace SmingCode.Utilities.Kafka.Consumers;

public interface IKafkaConsumerMiddleware
{
    Task<KafkaEventResult> HandleAsync<TKey, TValue>(
        KafkaConsumerContext<TKey, TValue> context,
        IKafkaConsumeDelegateHandler<TKey, TValue> kafkaConsumeDelegateHandler
    );
}

public delegate Task<KafkaEventResult> KafkaConsumeDelegate<TKey, TValue>(
    KafkaConsumerContext<TKey, TValue> context
);
