using Microsoft.Extensions.Logging;

namespace SmingCode.Utilities.ProcessTracking.WebApi;

internal class ProcessTrackingHeadersProvider(
    IProcessTrackingHandler _processTrackingHandler,
    ILogger<ProcessTrackingHeadersProvider> _logger
) : IProcessTrackingHeadersProvider, IProcessTrackingHeadersProviderInternal
{
    public ProcessTrackingDetail GetProcessTrackingDetailFromHeaders(
        HttpRequest httpRequest
    )
    {
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(
                "Incoming headers are: {HeaderInfo}",
                string.Join(
                    ",",
                    httpRequest.Headers.Select(header =>
                        $"{header.Key}:{header.Value}"
                    )
                )
            );            
        }

        if (!(
            httpRequest.Headers.TryGetValue(Configuration.PROCESS_ID_HEADER_NAME, out var processIdHeaderVal)
            && httpRequest.Headers.TryGetValue(Configuration.CORRELATION_ID_HEADER_NAME, out var correlationIdHeaderVal)
        ))
        {
            throw new Exception();
        }

        return new(
            processIdHeaderVal!,
            correlationIdHeaderVal!
        );
    }

    public Dictionary<string, string> GetProcessTrackingHeaders() =>
        _processTrackingHandler.TryGetProcessTrackingDetail(out var processTrackingDetail)
            ? new()
            {
                { Configuration.PROCESS_ID_HEADER_NAME, processTrackingDetail.ProcessId },
                { Configuration.CORRELATION_ID_HEADER_NAME, processTrackingDetail.CorrelationId }
            }
            : [];
}