namespace SmingCode.Utilities.ProcessTracking.WebApi;

internal interface IProcessTrackingHeadersProviderInternal
{
    ProcessTrackingDetail GetProcessTrackingDetailFromHeaders(
        HttpRequest httpRequest
    );
}
