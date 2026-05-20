using System.Diagnostics.CodeAnalysis;

namespace SmingCode.Utilities.ProcessTracking;

internal interface IProcessTrackingHandler
{
    bool IsConfigured { get; }
    ProcessTrackingDetail ProcessTrackingDetail { get; }
    Dictionary<string, object> ProcessTags { get; }
    void SetProcessTrackingDetail(ProcessTrackingDetail detail);
    ProcessTrackingDetail InitialiseNewProcess(
        string processName
    );
    bool TryLoadProcessDetailFromIncomingTags(
        IEnumerable<KeyValuePair<string, object>> incomingTags,
        [NotNullWhen(true)] out ProcessTrackingDetail? processTrackingDetail
    );
    Dictionary<string, object> StructuredLoggingMetadata { get; }
}