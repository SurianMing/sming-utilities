namespace SurianMing.Utilities.Kafka.MinimalConsumers;

public interface IMinimalConsumer
{
    void Consume(KafkaApplicationBuilder builder);
}
