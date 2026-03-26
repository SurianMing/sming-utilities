namespace SurianMing.Utilities.Kafka;

internal interface IKafkaConsumer
{
    void InitialiseEventConsumer(
        CancellationToken cancellationToken
    );
}
