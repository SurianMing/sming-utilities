using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace SmingCode.Utilities.ProcessTracking;
using Config;
using ServiceMetadata;

internal class ProcessTrackingHandler(
    IServiceMetadataProvider? serviceMetadataProvider,
    ILogger<ProcessTrackingHandler> _logger
) : IProcessTrackingHandler
{
    private ProcessTrackingDetail? _processTrackingDetail;
    private readonly Dictionary<string, object> _serviceMetadataCustomDimensions
        = serviceMetadataProvider?.GetMetadata().GetCustomDimensions() ?? [];

    public bool IsConfigured => _processTrackingDetail is not null;

    public ProcessTrackingDetail ProcessTrackingDetail =>
        _processTrackingDetail
            ?? throw new InvalidOperationException(
                "Attempt to access process tracking detail before it has been set. Please check IsConfigured first."
            );

    public ProcessTrackingDetail InitialiseNewProcess(
        string processName
    )
    {
        _processTrackingDetail = new(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            processName
        );

        if (_logger.IsEnabled(LogLevel.Information))
        {

            using var scope = _logger.BeginScope(
                StructuredLoggingMetadata
                    .Concat(_serviceMetadataCustomDimensions)
            );

            _logger.LogInformation(
                "New process initialised - {TraceType}",
                Constants.PROCESS_TRACKING_UTILITY_TRACE_TYPE
            );
        }

        return _processTrackingDetail;
    }

    public bool TryLoadProcessDetailFromIncomingTags(
        IEnumerable<KeyValuePair<string, object>> incomingTags,
        [NotNullWhen(true)] out ProcessTrackingDetail? processTrackingDetail
    )
    {
        var tagDictionary = incomingTags.ToDictionary();

        if (tagDictionary.TryGetValue(Constants.CORRELATION_ID_TAG_NAME, out var correlationId)
            && tagDictionary.TryGetValue(Constants.PROCESS_ID_TAG_NAME, out var processId)
            && tagDictionary.TryGetValue(Constants.PROCESS_NAME_TAG_NAME, out var processName)
            && !(string.IsNullOrEmpty(correlationId.ToString()) || string.IsNullOrEmpty(processId.ToString()) || string.IsNullOrEmpty(processName.ToString())))
        {
            _processTrackingDetail = new(
                correlationId.ToString()!,
                processId.ToString()!,
                processName.ToString()!
            );
            processTrackingDetail = _processTrackingDetail;

            if (_logger.IsEnabled(LogLevel.Information))
            {

                using var scope = _logger.BeginScope(
                    StructuredLoggingMetadata
                        .Concat(_serviceMetadataCustomDimensions)
                );

                _logger.LogInformation(
                    "Process details retrieved from incoming metadata - {TraceType}",
                    Constants.PROCESS_TRACKING_UTILITY_TRACE_TYPE
                );
            }

            return true;
        }

        processTrackingDetail = null;
        return false;
    }

    public Dictionary<string, object> ProcessTags =>
        new ()
        {
            { Constants.CORRELATION_ID_TAG_NAME, ProcessTrackingDetail.CorrelationId },
            { Constants.PROCESS_ID_TAG_NAME, ProcessTrackingDetail.ProcessId },
            { Constants.PROCESS_NAME_TAG_NAME, ProcessTrackingDetail.ProcessName }
        };

    public void SetProcessTrackingDetail(
        ProcessTrackingDetail detail
    ) => _processTrackingDetail = detail;

    public Dictionary<string, object> StructuredLoggingMetadata
        => new()
        {
            { Constants.CORRELATION_ID_STRUCTURED_LOGGING_METADATA_KEY, ProcessTrackingDetail.CorrelationId },
            { Constants.PROCESS_ID_STRUCTURED_LOGGING_METADATA_KEY, ProcessTrackingDetail.ProcessId },
            { Constants.PROCESS_NAME_STRUCTURED_LOGGING_METADATA_KEY, ProcessTrackingDetail.ProcessName }
        };
}
