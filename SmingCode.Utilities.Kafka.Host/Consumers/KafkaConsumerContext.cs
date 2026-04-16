namespace SmingCode.Utilities.Kafka.Host.Consumers;

public class KafkaConsumerContext<TKey, TValue>(
    string topicConsumed,
    ConsumeResult<TKey, TValue> consumeResult,
    IServiceProvider serviceProvider
)
{
    public string TopicConsumed { get; private set; } = topicConsumed;
    public ConsumeResult<TKey, TValue> ConsumeResult { get; private set; } = consumeResult;
    public IServiceProvider ServiceProvider { get; private set; } = serviceProvider;
}
