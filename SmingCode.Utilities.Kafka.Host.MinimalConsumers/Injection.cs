using System.Reflection;

namespace SmingCode.Utilities.Kafka.Host.MinimalConsumers;

public static class Injection
{
    public static KafkaApplicationBuilder LoadConsumers(
        this KafkaApplicationBuilder builder
    ) => builder.LoadConsumers(Assembly.GetCallingAssembly());

    public static KafkaApplicationBuilder LoadConsumers<TConsumerLocator>(
        this KafkaApplicationBuilder builder
    ) => builder.LoadConsumers(
        Assembly.GetAssembly(typeof(TConsumerLocator))
            ?? throw new InvalidOperationException(
                $"No loaded assembly contains the Consumer {typeof(TConsumerLocator).Name}."
            ));

    public static KafkaApplicationBuilder LoadConsumers(
        this KafkaApplicationBuilder builder,
        Assembly consumersAssembly
    )
    {
        var consumerImplementations = consumersAssembly
            .GetTypes()
            .Where(type =>
                typeof(IMinimalConsumer).IsAssignableFrom(type)
                && type is { IsInterface: false, IsAbstract: false }
            )
            .Select(Activator.CreateInstance)
            .OfType<IMinimalConsumer>()
            .ToList();

        consumerImplementations.ForEach(consumer =>
            consumer.Consume(builder)
        );

        return builder;
    }
}