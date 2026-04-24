namespace SmingCode.Utilities.ServiceApiClient;

public class ApiClientSendContext<TBody, TResponse>(
    HttpMethod httpMethod,
    string targetUrl,
    TBody body,
    HeaderEntryCollection messageHeaders,
    IServiceProvider serviceProvider
)
{
    public HttpMethod HttpMethod { get; } = httpMethod;
    public string TargetUrl { get; private set; } = targetUrl;
    public TBody Body { get; } = body;
    public HeaderEntryCollection MessageHeaders { get; } = messageHeaders;
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public void UpdateTargetUrl(string newTargetUrl) => TargetUrl = newTargetUrl;
}
