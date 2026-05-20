namespace SmingCode.Utilities.ProcessTracking.Kafka.Config;

using SmingCode.Utilities.Kafka.Consumers;
using Utilities.Kafka.Config;

public static class Injection
{
    public static IValidProcessTrackingBuilder AddKafkaMiddleware(
        this IProcessTrackingBuilder builder
    )
    {
        var services = ((IProcessTrackingBuilderInternal)builder).Services;
        services.AddKafkaConsumerMiddleware<ProcessTrackingConsumerMiddleware>();
        services.AddKafkaProducerMiddleware<ProcessTrackingProducerMiddleware>();

        return new ValidProcessTrackingBuilder(services);
    }

    public static IKafkaConsumerDefinition InitialisesProcess(
        this IKafkaConsumerDefinition kafkaConsumerDefinition,
        string processName
    )
    {
        kafkaConsumerDefinition.CustomPropertyHandler.TryAddCustomProperty(
            Constants.INITIALISES_PROCESS_CUSTOM_PROPERTY_NAME, true
        );
        kafkaConsumerDefinition.CustomPropertyHandler.TryAddCustomProperty(
            Constants.PROCESS_NAME_CUSTOM_PROPERTY_NAME, processName
        );

        return kafkaConsumerDefinition;
    }
}