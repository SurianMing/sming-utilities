namespace SmingCode.Utilities.Kafka;

public interface IKafkaConsumerDefinition
{
    IKafkaConsumerDefinition WithIsolationMode(
        IsolationMode isolationMode
    );
    IKafkaConsumerDefinition UseRegexPatternMatchingForTopic();
    IKafkaConsumerDefinition CreateTopicIfNotExists();
}
