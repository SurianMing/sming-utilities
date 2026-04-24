namespace SmingCode.Utilities.Kafka.Consumers;

internal class KafkaConsumeDelegateHandler<TKey, TValue>(
    KafkaConsumeDelegate<TKey, TValue> _delegate
) : IKafkaConsumeDelegateHandler<TKey, TValue>
{
    public async Task<KafkaEventResult> Next(KafkaConsumerContext<TKey, TValue> context)
        => await _delegate(context);
}