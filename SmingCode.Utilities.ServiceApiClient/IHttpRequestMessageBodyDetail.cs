namespace SmingCode.Utilities.ServiceApiClient;

internal interface IHttpRequestMessageBodyDetail
{
    StringContent StringContent { get; }
    object BodyForLogging { get; }
}
