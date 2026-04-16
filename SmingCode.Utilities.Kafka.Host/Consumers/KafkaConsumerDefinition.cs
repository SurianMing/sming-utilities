namespace SmingCode.Utilities.Kafka.Host.Consumers;

internal class KafkaConsumerDefinition<TKey, TValue>(
    string topicToMatch,
    Delegate handler
) : IKafkaConsumerDefinition
{
    internal KafkaDelegateInvoker<TKey, TValue> Handler { get; } = new(handler);
    internal IsolationMode IsolationMode { get; private set; } = IsolationMode.PerServiceType;
    internal bool UseRegexPatternMatching { get; private set; }
    internal bool CreateTopic { get; private set; }

    public string TopicToMatch { get; } = topicToMatch;

    public IKafkaConsumerDefinition WithIsolationMode(
        IsolationMode isolationMode
    )
    {
        IsolationMode = isolationMode;

        return this;
    }

    public IKafkaConsumerDefinition UseRegexPatternMatchingForTopic()
    {
        UseRegexPatternMatching = true;

        return this;
    }

    public IKafkaConsumerDefinition CreateTopicIfNotExists()
    {
        CreateTopic = true;

        return this;
    }
}
