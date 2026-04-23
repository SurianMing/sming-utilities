using Microsoft.Extensions.Logging;

namespace SmingCode.Utilities.ProcessTracking.WebApi;
using SmingCode.Utilities.ServiceApiClient;

internal class ProcessTrackingHttpClientInjection(
    IProcessTrackingHandler _processTrackingHandler,
    ILogger<ProcessTrackingHttpClientInjection> _logger
) : IDelegateHandlingExtension
{
    public HttpRequestMessage Handle(
        HttpRequestMessage requestMessage
    )
    {
        var currentProcessTags = _processTrackingHandler.ProcessTags;

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(
                "Adding process tags to http message: {ProcessTags}",
                string.Join(
                    ",",
                    currentProcessTags.Select(header =>
                        $"{header.Key}:{header.Value}"
                    )
                )
            );
        }

        foreach (var processTag in currentProcessTags)
        {
            requestMessage.Headers.Add(processTag.Key, processTag.Value.ToString());
        }

        return requestMessage;
    }
}
