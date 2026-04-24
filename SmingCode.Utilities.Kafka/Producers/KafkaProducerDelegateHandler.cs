namespace SmingCode.Utilities.Kafka.Producers;

internal class KafkaProducerDelegateHandler<TKey, TValue>(
    KafkaProducerDelegate<TKey, TValue> _delegate
) : IKafkaProducerDelegateHandler<TKey, TValue>
{
    public async Task<bool> Next(
        IKafkaProducerContext<TKey, TValue> context
    ) => await _delegate(context);
}