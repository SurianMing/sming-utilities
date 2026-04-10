using System.Diagnostics.CodeAnalysis;

namespace SmingCode.Utilities.ProcessTracking;

internal interface IProcessTrackingHandler
{
    bool TryGetProcessTrackingDetail(
        [NotNullWhen(true)] out ProcessTrackingDetail? processTrackingDetail
    );
    void SetProcessTrackingDetail(ProcessTrackingDetail detail);
}
