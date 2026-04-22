namespace SmingCode.Utilities.ProcessTracking.Kafka;
using Utilities.Kafka.Consumers;
using Utilities.Kafka.Producers;

public static class Injection
{
    public static IValidProcessTrackingBuilder AddKafkaMiddleware(
        this IProcessTrackingBuilder builder
    )
    {
        var services = ((IProcessTrackingBuilderInternal)builder).Services;
        services.AddSingleton<IKafkaConsumerMiddleware, ProcessTrackingConsumerMiddleware>();
        services.AddSingleton<IKafkaProducerMiddleware, ProcessTrackingProducerMiddleware>();

        return new ValidProcessTrackingBuilder(services);
    }
}