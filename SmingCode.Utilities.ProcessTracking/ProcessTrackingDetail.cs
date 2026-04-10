namespace SmingCode.Utilities.ProcessTracking;

internal record ProcessTrackingDetail(
    string CorrelationId,
    string ProcessId
);

internal static class ProcessTrackingDetailExtensions
{
    internal static List<KeyValuePair<string, object>> GetActivityTags(
        this ProcessTrackingDetail processTrackingDetail
    ) => [
        new("correlation-id", processTrackingDetail.CorrelationId),
        new("process-id", processTrackingDetail.ProcessId)
    ];
}