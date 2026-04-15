namespace SmingCode.Utilities.Kafka.Consumers;

internal interface IKafkaConsumer
{
    void InitialiseEventConsumer(
        CancellationToken cancellationToken
    );
}
