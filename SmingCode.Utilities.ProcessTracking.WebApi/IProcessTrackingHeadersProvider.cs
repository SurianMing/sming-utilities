namespace SmingCode.Utilities.ProcessTracking.WebApi;

public interface IProcessTrackingHeadersProvider
{
    Dictionary<string, string> GetProcessTrackingHeaders();
}
