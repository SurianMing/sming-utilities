namespace SmingCode.Utilities.ProcessTracking;

internal class ProcessTrackingHandlerManager(
    IEnumerable<IProcessTrackingHandler> _processTrackingHandlers
)
{
    internal ProcessTrackingDetail GetProcessTrackingDetail()
    {
        foreach (var handler in _processTrackingHandlers)
        {
            if (handler.TryGetProcessTrackingDetail(out var processTrackingDetail))
            {
                return processTrackingDetail;
            }
        }

        throw new Exception();
    }
}