namespace SmingCode.Utilities.ServiceApiClient;

internal class ApiClientSendDelegateHandler<TBody, TResponse>(
    ApiClientSendDelegate<TBody, TResponse> _delegate
) : IApiClientSendDelegateHandler<TBody, TResponse>
{
    public async Task<TResponse> Next(ApiClientSendContext<TBody, TResponse> context)
        => await _delegate(context);
}

internal Func<TBody, TResponse> Task SendDelegate<TBody, TResponse>(
    ApiClientSendContext<TBody, TResponse> context
);