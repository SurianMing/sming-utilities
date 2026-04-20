using System.Diagnostics.CodeAnalysis;

namespace SmingCode.Utilities.ProcessTracking;

internal class ProcessTrackingHandler : IProcessTrackingHandler
{
    private const string CORRELATION_ID_TAG_NAME = "correlation-id";
    private const string CORRELATION_ID_STRUCTURED_LOGGING_METADATA_KEY = "CorrelationId";
    private const string PROCESS_ID_TAG_NAME = "process-id";
    private const string PROCESS_ID_STRUCTURED_LOGGING_METADATA_KEY = "ProcessId";

    private ProcessTrackingDetail? _processTrackingDetail;

    public bool IsConfigured => _processTrackingDetail is not null;

    public ProcessTrackingDetail ProcessTrackingDetail =>
        _processTrackingDetail
            ?? throw new InvalidOperationException(
                "Attempt to access process tracking detail before it has been set. Please check IsConfigured first."
            );

    public bool TryLoadProcessDetailFromIncomingTags(
        IEnumerable<KeyValuePair<string, object>> incomingTags,
        [NotNullWhen(true)] out ProcessTrackingDetail? processTrackingDetail
    )
    {
        var tagDictionary = incomingTags.ToDictionary();

        if (tagDictionary.TryGetValue(CORRELATION_ID_TAG_NAME, out var correlationId)
            && tagDictionary.TryGetValue(PROCESS_ID_TAG_NAME, out var processId)
            && !(string.IsNullOrEmpty(correlationId.ToString()) || string.IsNullOrEmpty(processId.ToString())))
        {
            _processTrackingDetail = new(
                correlationId.ToString()!,
                processId.ToString()!
            );
            processTrackingDetail = _processTrackingDetail;
            return true;
        }

        processTrackingDetail = null;
        return false;
    }

    public Dictionary<string, object> ProcessTags =>
        new ()
        {
            { CORRELATION_ID_TAG_NAME, ProcessTrackingDetail.CorrelationId },
            { PROCESS_ID_TAG_NAME, ProcessTrackingDetail.ProcessId }
        };

    public void SetProcessTrackingDetail(
        ProcessTrackingDetail detail
    ) => _processTrackingDetail = detail;

    public Dictionary<string, object> StructuredLoggingMetadata
        => new()
        {
            { CORRELATION_ID_STRUCTURED_LOGGING_METADATA_KEY, ProcessTrackingDetail.CorrelationId },
            { PROCESS_ID_STRUCTURED_LOGGING_METADATA_KEY, ProcessTrackingDetail.ProcessId }
        };
}
