using System.Reflection;
using Microsoft.Extensions.Hosting;
using SurianMing.Utilities.ApplicationSettings;

namespace SurianMing.Utilities.Kafka;

internal class KafkaApplication(
    IEnumerable<IKafkaConsumerDefinition> kafkaConsumerDefinitions,
    IServiceProvider serviceProvider,
    ILogger<KafkaApplication> _logger
) : BackgroundService
{
    private readonly List<IKafkaConsumer> _kafkaConsumers = [.. kafkaConsumerDefinitions
        .Select(definition =>
        {
            var consumerType = typeof(KafkaConsumer<,>)
                .MakeGenericType(definition.GetType().GetGenericArguments());

            return (IKafkaConsumer)ActivatorUtilities.CreateInstance(
                serviceProvider,
                consumerType,
                [ definition ]
            );
        })];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _kafkaConsumers.ForEach(consumer => consumer.InitialiseEventConsumer(stoppingToken));

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
