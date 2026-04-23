using Microsoft.Extensions.Logging;

namespace SmingCode.Utilities.ProcessTracking.WebApi;
using ServiceMetadata;

internal class WebApiIngressMiddleware(
    RequestDelegate _next,
    IServiceMetadataProvider serviceMetadataProvider,
    ILogger<WebApiIngressMiddleware> _logger
)
{
    public async Task InvokeAsync(
        HttpContext httpContext,
        IProcessTrackingHandler processTrackingHandler
    )
    {
        var headers = httpContext.Request.Headers;

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(
                "Incoming headers are: {HeaderInfo}",
                string.Join(
                    ",",
                    headers.Select(header =>
                        $"{header.Key}:{header.Value}"
                    )
                )
            );
        }

        if (!processTrackingHandler.TryLoadProcessDetailFromIncomingTags(
            headers.ToDictionary(
                header => header.Key,
                header => (object)header.Value
            ),
            out var processTrackingDetail
        ))
        {
            throw new Exception();
        }

        using var scope = _logger.BeginScope(
            processTrackingHandler.StructuredLoggingMetadata
                .Concat(serviceMetadataProvider.GetMetadata().GetCustomDimensions())
        );

        await _next(httpContext);
    }
}
