using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmingCode.Utilities.Kafka.Producers;

namespace SmingCode.Utilities.Kafka;

public static class Injection
{
    public static IServiceCollection AddKafkaProducing(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var kafkaServerOptions = configuration.GetRequiredSection("Kafka")
            .Get<KafkaServerOptions>()
            ?? throw new InvalidOperationException("No valid kafka configuration section found.");
        services.TryAddSingleton(kafkaServerOptions);

        services.TryAddScoped<IKafkaProducer, KafkaProducer>();

        return services;
    }
}