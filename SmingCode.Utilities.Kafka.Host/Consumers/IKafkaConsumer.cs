namespace SmingCode.Utilities.Kafka.Host.Consumers;

internal interface IKafkaConsumer
{
    void InitialiseEventConsumer(
        CancellationToken cancellationToken
    );
}
