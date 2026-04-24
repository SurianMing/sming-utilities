using System.Text;
using System.Text.Json;

namespace SmingCode.Utilities.ServiceApiClient;

public class RequestBody<TBody>(
    TBody _body,
    JsonSerializerOptions _jsonSerializerOptions
) : IHttpRequestMessageBodyDetail where TBody : notnull
{
    private static readonly Encoding _encoding = Encoding.UTF8;
    private static readonly string _jsonMediaType = "application/json";
    private static readonly string _plainTextMediaType = "text/plain";
    private readonly bool _isStringType = typeof(TBody) == typeof(string);
    private string? _contentType;

    private string ContentType => _contentType
        ??= _isStringType ? _plainTextMediaType : _jsonMediaType;

    public StringContent StringContent => new(
        _isStringType
            ? _body.ToString()!
            : JsonSerializer.Serialize(
                _body,
                _jsonSerializerOptions
            ),
        _encoding,
        ContentType
    );

    public object BodyForLogging => new
    {
        Body = _body,
        Encoding = _encoding.EncodingName,
        ContentType
    };
}
