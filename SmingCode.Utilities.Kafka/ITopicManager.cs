namespace SmingCode.Utilities.Kafka;

internal interface ITopicManager
{
    Task<bool> CreateTopic(
        string topicName,
        short replicationFactor = 1
    );
    Task<bool> RemoveTopic(
        string topicName
    );
}
