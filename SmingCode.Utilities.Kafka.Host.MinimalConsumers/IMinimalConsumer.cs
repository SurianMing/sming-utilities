namespace SmingCode.Utilities.Kafka.Host.MinimalConsumers;

public interface IMinimalConsumer
{
    void Consume(KafkaApplicationBuilder builder);
}
