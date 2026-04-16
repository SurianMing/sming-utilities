using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;

namespace SmingCode.Utilities.Kafka.Host;
using Consumers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Producers;
using ServiceMetadata;

public class KafkaApplicationBuilder(
    KafkaApplicationBuilderSettings? settings
) : IHostApplicationBuilder
{
    private readonly HostApplicationBuilder _hostApplicationBuilder = new (
        settings?.ToHostApplicationBuilderSettings()
    );

    public KafkaApplicationBuilder()
        : this(args: null) { }

    public KafkaApplicationBuilder(string[]? args)
        : this(new KafkaApplicationBuilderSettings { Args = args })
    { }

    public IHostEnvironment Environment => _hostApplicationBuilder.Environment;
    public ConfigurationManager Configuration => _hostApplicationBuilder.Configuration;
    IConfigurationManager IHostApplicationBuilder.Configuration => Configuration;
    public IServiceCollection Services => _hostApplicationBuilder.Services;
    public ILoggingBuilder Logging => _hostApplicationBuilder.Logging;
    public IMetricsBuilder Metrics => _hostApplicationBuilder.Metrics;

    public void ConfigureContainer<TContainerBuilder>(
        IServiceProviderFactory<TContainerBuilder> factory,
        Action<TContainerBuilder>? configure = null
    ) where TContainerBuilder : notnull
        => _hostApplicationBuilder.ConfigureContainer(factory, configure);

    public IDictionary<object, object> Properties => ((IHostApplicationBuilder)_hostApplicationBuilder).Properties;
    IDictionary<object, object> IHostApplicationBuilder.Properties => throw new NotImplementedException();

    public IHost Build()
    {
        var kafkaServerOptions = Configuration.GetRequiredSection("Kafka")
            .Get<KafkaServerOptions>()
            ?? throw new InvalidOperationException("No valid kafka configuration section found.");
        Services.TryAddSingleton(kafkaServerOptions);
        Services.AddSingleton<IAdminClientProvider, AdminClientProvider>();
        Services.AddSingleton<ITopicManager, TopicManager>();
        Services.TryAddScoped<IKafkaProducer, KafkaProducer>();
        Services.InitializeServiceMetadata();
        Services.AddHostedService<KafkaApplication>();

        return _hostApplicationBuilder.Build();
    }

    public IKafkaConsumerDefinition MapConsumer(
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
        Services.AddSingleton(newKafkaConsumerDefinition);

        return newKafkaConsumerDefinition;
    }
}
