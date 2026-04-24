namespace SmingCode.Utilities.ServiceApiClient;

public class ServiceApiClientException(
    string targetServiceName,
    string message,
    Exception? ex = null,
    HttpResponseMessage? _responseMessage = null
) : Exception($"Exception occurred in service api client targeting service {targetServiceName} - {message}", ex)
{
    public bool CausedByUnsuccessfulResponseMessage => _responseMessage is not null;

    public HttpResponseMessage UnsuccessfulResponseMessage => _responseMessage
        ?? throw new InvalidOperationException(
            $"Attempt to retrieve unsuccessful response message when none exists. Please check {(nameof(CausedByUnsuccessfulResponseMessage))} first."
        );
}