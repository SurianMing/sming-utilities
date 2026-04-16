namespace SmingCode.Utilities.Kafka.Host.Consumers;

public interface IKafkaConsumerDefinition
{
    IKafkaConsumerDefinition WithIsolationMode(
        IsolationMode isolationMode
    );
    IKafkaConsumerDefinition UseRegexPatternMatchingForTopic();
    IKafkaConsumerDefinition CreateTopicIfNotExists();
}
