using System.Diagnostics.CodeAnalysis;

namespace SmingCode.Utilities.ProcessTracking;

internal class ProcessTrackingHandler : IProcessTrackingHandler
{
    private ProcessTrackingDetail? _processTrackingDetail;

    public void SetProcessTrackingDetail(
        ProcessTrackingDetail detail
    ) => _processTrackingDetail = detail;

    public bool TryGetProcessTrackingDetail(
        [NotNullWhen(true)] out ProcessTrackingDetail? processTrackingDetail
    )
    {
        processTrackingDetail = _processTrackingDetail;

        return processTrackingDetail is not null;
    }
}
