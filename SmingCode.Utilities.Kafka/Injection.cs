using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmingCode.Utilities.Kafka.Consumers;
using SmingCode.Utilities.Kafka.Producers;
using SmingCode.Utilities.ServiceMetadata;

namespace SmingCode.Utilities.Kafka;

public static class Injection
{
    public static IKafkaConsumerDefinition MapConsumer(
        this IServiceCollection services,
        string topicToMatch,
        Delegate handler
    )
    {
        var handlerMethodParameters = handler.Method.GetParameters();
        var consumerKeyType = handlerMethodParameters
            .SingleOrDefault(parameter => parameter.GetCustomAttribute<FromEventKeyAttribute>() is not null)
                ?.ParameterType
                ?? typeof(Ignore);
        var consumerValueType = handlerMethodParameters
            .SingleOrDefault(parameter => parameter.GetCustomAttribute<FromEventValueAttribute>() is not null)
                ?.ParameterType
                ?? typeof(Ignore);

        var kafkaConsumerDefinitionType = typeof(KafkaConsumerDefinition<,>);
        var typedKafkaConsumerDefinitionType = kafkaConsumerDefinitionType
            .MakeGenericType(consumerKeyType, consumerValueType);

        var newKafkaConsumerDefinition = (IKafkaConsumerDefinition)Activator.CreateInstance(
            typedKafkaConsumerDefinitionType,
            [ topicToMatch, handler ]
        )!;
        services.AddSingleton(newKafkaConsumerDefinition);

        return newKafkaConsumerDefinition;
    }

    public static IServiceCollection InitializeKafkaHandling(
        this IServiceCollection services,
        IConfiguration configuration,
        bool includeConsumers = false
    )
    {
        var kafkaServerOptions = configuration.GetRequiredSection("Kafka")
            .Get<KafkaServerOptions>()
            ?? throw new InvalidOperationException("No valid kafka configuration section found.");
        services.AddSingleton(kafkaServerOptions);
        services.AddSingleton<IAdminClientProvider, AdminClientProvider>();
        services.AddSingleton<ITopicManager, TopicManager>();
        services.AddScoped<IKafkaProducer, KafkaProducer>();
        if (includeConsumers)
        {
            services.AddHostedService<KafkaHostedService>();
        }

        return services;
    }
}