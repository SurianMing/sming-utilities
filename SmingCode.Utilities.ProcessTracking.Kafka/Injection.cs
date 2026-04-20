using Microsoft.Extensions.DependencyInjection;
using SmingCode.Utilities.Kafka.Consumers;

namespace SmingCode.Utilities.ProcessTracking.Kafka;

public static class Injection
{
    public static IValidProcessTrackingBuilder AddKafkaMiddleware(
        this IProcessTrackingBuilder builder
    )
    {
        var services = ((IProcessTrackingBuilderInternal)builder).Services;
        services.AddScoped<IKafkaConsumerMiddleware, ProcessTrackingConsumerMiddleware>();

        return new ValidProcessTrackingBuilder(services);
    }
}