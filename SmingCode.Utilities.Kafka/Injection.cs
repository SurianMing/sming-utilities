using Microsoft.Extensions.Configuration;
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
        services.AddSingleton(kafkaServerOptions);

        services.AddScoped<IKafkaProducer, KafkaProducer>();

        return services;
    }
}