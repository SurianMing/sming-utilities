namespace SmingCode.Utilities.ServiceApiClient;

public interface IApiClientSendDelegateHandler<TBody, TResponse>
{
    Task<TResponse> Next(
        ApiClientSendContext<TBody, TResponse> context
    );
}
