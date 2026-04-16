namespace SmingCode.Utilities.Kafka.Host.Consumers;

public interface IKafkaConsumerMiddleware
{
    Task<KafkaEventResult> HandleAsync<TKey, TValue>(
        KafkaConsumerContext<TKey, TValue> context,
        KafkaConsumeDelegate<TKey, TValue> kafkaConsumeDelegate
    );
}

public delegate Task<KafkaEventResult> KafkaConsumeDelegate<TKey, TValue>(
    KafkaConsumerContext<TKey, TValue> context
);
