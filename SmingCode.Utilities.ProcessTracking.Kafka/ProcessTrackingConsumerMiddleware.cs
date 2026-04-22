using System.Text;
using Microsoft.Extensions.Logging;

namespace SmingCode.Utilities.ProcessTracking.Kafka;
using ServiceMetadata;
using Utilities.Kafka.Consumers;

internal class ProcessTrackingConsumerMiddleware(
    IServiceMetadataProvider serviceMetadataProvider,
    ILogger<ProcessTrackingConsumerMiddleware> _logger
) : IKafkaConsumerMiddleware
{
    public async Task<KafkaEventResult> HandleAsync<TKey, TValue>(
        KafkaConsumerContext<TKey, TValue> context,
        KafkaConsumeDelegate<TKey, TValue> kafkaConsumeDelegate
    )
    {
        var messageHeaders = context.ConsumeResult.Message.Headers
            .ToDictionary(
                header => header.Key,
                header => Encoding.UTF8.GetString(header.GetValueBytes())
            );

        var processTrackingHandler = context.ServiceProvider.GetRequiredService<IProcessTrackingHandler>();

        if (!processTrackingHandler.TryLoadProcessDetailFromIncomingTags(
            messageHeaders.ToDictionary(
                header => header.Key,
                header => (object)header.Value
            ),
            out var processTrackingDetail
        ))
        {
            _logger.LogError(
                "Unable to load process tracking headers. Marking message as unprocessed"
            );

            return KafkaEventResult.Incomplete;
        }

        using var scope = _logger.BeginScope(
            processTrackingHandler.StructuredLoggingMetadata
                .Concat(serviceMetadataProvider.GetMetadata().GetCustomDimensions())
        );

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(
                "Process tracking details loaded from incoming message headers."
            );
        }

        var result = await kafkaConsumeDelegate(
            context
        );

        return result;
    }
}
