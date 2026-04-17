namespace SmingCode.Utilities.Kafka.Consumers;

public interface IKafkaConsumerDefinition
{
    IKafkaConsumerDefinition WithIsolationMode(
        IsolationMode isolationMode
    );
    IKafkaConsumerDefinition UseRegexPatternMatchingForTopic();
    IKafkaConsumerDefinition CreateTopicIfNotExists();
}
