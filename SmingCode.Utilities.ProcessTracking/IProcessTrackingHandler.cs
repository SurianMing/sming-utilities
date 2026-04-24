using System.Diagnostics.CodeAnalysis;

namespace SmingCode.Utilities.ProcessTracking;

public interface IProcessTrackingHandler
{
    bool IsConfigured { get; }
    ProcessTrackingDetail ProcessTrackingDetail { get; }
    Dictionary<string, object> ProcessTags { get; }
    void SetProcessTrackingDetail(ProcessTrackingDetail detail);
    bool TryLoadProcessDetailFromIncomingTags(
        IEnumerable<KeyValuePair<string, object>> incomingTags,
        [NotNullWhen(true)] out ProcessTrackingDetail? processTrackingDetail
    );
    Dictionary<string, object> StructuredLoggingMetadata { get; }
}
