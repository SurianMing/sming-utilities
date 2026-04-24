namespace SmingCode.Utilities.ServiceApiClient;

public interface IApiClientSendMiddleware
{
    Task<TResponse> HandleAsync<TBody, TResponse>(
        ApiClientSendContext<TBody, TResponse> context,
        IApiClientSendDelegateHandler<TBody, TResponse> apiClientSendDelegate
    );
}

public delegate Task<TResponse> ApiClientSendDelegate<TBody, TResponse>(
    ApiClientSendContext<TBody, TResponse> context
);
