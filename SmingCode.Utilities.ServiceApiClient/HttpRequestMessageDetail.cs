using System.Text.Json;

namespace SmingCode.Utilities.ServiceApiClient;

internal class HttpRequestMessageDetail(
    HttpMethod _httpMethod,
    string _requestUri,
    HeaderEntryCollection? _headers,
    IHttpRequestMessageBodyDetail? _bodyDetail = null
)
{
    internal HttpRequestMessage HttpRequestMessage
    {
        get
        {
            var httpRequestMessage = new HttpRequestMessage(
                _httpMethod,
                _requestUri
            );

            _headers?.Headers.ForEach(header =>
                httpRequestMessage.Headers.Add(header.Key, header.Value)
            );

            if (_bodyDetail is not null)
            {
                httpRequestMessage.Content = _bodyDetail.StringContent;
            }

            return httpRequestMessage;            
        }
    }

    internal string LogDetail
    {
        get
        {
            var logDetail = new
            {
                HttpMethod = _httpMethod.Method,
                TargetUrl = _requestUri,
                Headers = _headers ?? [],
                Body = _bodyDetail?.BodyForLogging
            };

            return JsonSerializer.Serialize(logDetail);
        }
    }
}
