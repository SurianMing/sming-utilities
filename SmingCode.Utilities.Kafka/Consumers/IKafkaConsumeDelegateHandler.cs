namespace SmingCode.Utilities.Kafka.Consumers;

public interface IKafkaConsumeDelegateHandler<TKey, TValue>
{
    Task<KafkaEventResult> Next(
        KafkaConsumerContext<TKey, TValue> context
    );
}
