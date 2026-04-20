using Microsoft.Extensions.Logging;

namespace SmingCode.Utilities.ProcessTracking.Kafka;

using System.Text;
using Microsoft.Extensions.DependencyInjection;
using ServiceMetadata;
using SmingCode.Utilities.Kafka.Consumers;

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


        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(
                "Incoming headers are: {HeaderInfo}",
                string.Join(
                    ",",
                    messageHeaders.Select(messageHeader =>
                        $"{messageHeader.Key}:{messageHeader.Value}"
                    )
                )
            );
        }

        var processTrackingHandler = context.ServiceProvider.GetRequiredService<IProcessTrackingHandler>();

        if (!processTrackingHandler.TryLoadProcessDetailFromIncomingTags(
            messageHeaders.ToDictionary(
                header => header.Key,
                header => (object)header.Value
            ),
            out var processTrackingDetail
        ))
        {
            throw new Exception();
        }

        using var scope = _logger.BeginScope(
            processTrackingHandler.StructuredLoggingMetadata
                .Concat(serviceMetadataProvider.GetMetadata().GetCustomDimensions())
        );

        var result = await kafkaConsumeDelegate(
            context
        );

        return result;
    }
}
