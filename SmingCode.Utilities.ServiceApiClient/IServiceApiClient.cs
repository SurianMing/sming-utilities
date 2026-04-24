namespace SmingCode.Utilities.ServiceApiClient;

public interface IServiceApiClient<TService>
    where TService : class
{
    HttpClient HttpClient { get; }
    Task Post(
        string relativeUrl
    );
    Task<TResult> Post<TRequest, TResult>(
        string relativeUrl,
        TRequest request,
        HeaderEntryCollection? headers = null
    ) where TRequest : notnull where TResult : notnull;
    Task<TResult> Get<TResult>(
        string relativeUrl
    ) where TResult : notnull;
}
