using Microsoft.Extensions.Logging;

namespace SmingCode.Utilities.ProcessTracking.Kafka;
using Utilities.Kafka.Producers;

internal class ProcessTrackingProducerMiddleware(
    ILogger<ProcessTrackingProducerMiddleware> _logger
) : IKafkaProducerMiddleware
{
    public async Task<bool> HandleAsync<TKey, TValue>(
        IKafkaProducerContext<TKey, TValue> context,
        IKafkaProducerDelegateHandler<TKey, TValue> kafkaProduceDelegateHandler
    )
    {
        var processTrackingHandler = context.ServiceProvider
            .GetRequiredService<IProcessTrackingHandler>();
        var processTrackingTags = processTrackingHandler.ProcessTags;

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(
                "Process tags being added to outgoing kafka message: {ProcessTagsRequired}",
                string.Join(
                    ",",
                    processTrackingTags.Select(tag =>
                        $"{tag.Key}:{tag.Value}"
                    )
                )
            );
        }
        
        foreach (var tag in processTrackingTags)
        {
            context.AddHeader(tag.Key, tag.Value.ToString()!);
        }

        var result = await kafkaProduceDelegateHandler.Next(
            context
        );

        return result;
    }
}
