using System.Text.Json;

namespace SurianMing.Utilities.Kafka;

internal class KafkaEventSender(
    KafkaServerOptions _kafkaServerOptions
) : IKafkaEventSender
{
    public async Task<bool> SendEvent<TEvent>(TEvent eventToSend)
        where TEvent : KafkaEvent
    {
        using var producer = GetProducer();
        var deliveryResult = await producer.ProduceAsync(
            eventToSend.Topic,
            new Message<Ignore, string> { Value = JsonSerializer.Serialize(eventToSend) }
        );

        return deliveryResult.Status == PersistenceStatus.Persisted;
    }

    public async Task<bool> SendEvent<TEvent>(TEvent eventToSend, Guid identifier)
        where TEvent : KafkaEvent
    {
        using var producer = GetProducer();
        var deliveryResult = await producer.ProduceAsync(
            eventToSend.Topic,
            new Message<Ignore, string>
            {
                Value = JsonSerializer.Serialize(eventToSend),
                Headers = new Headers
                {
                    new("CorrelationId", identifier.ToByteArray())
                }
            }
        );

        return deliveryResult.Status == PersistenceStatus.Persisted;
    }

    private IProducer<Ignore, string> GetProducer()
    {
        var producerBuilder = new ProducerBuilder<Ignore, string>(
            new ProducerConfig
            {
                BootstrapServers = $"{_kafkaServerOptions.BootstrapServers}",
                ApiVersionRequest = false,
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 1000,
                Acks = Acks.All
            });

        return producerBuilder.Build();
    }
}