using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace SmingCode.Utilities.ProcessTracking.WebApi;
using ServiceMetadata;

internal class ProcessTrackingHeaderMiddleware(
    RequestDelegate _next,
    IServiceMetadataProvider serviceMetadataProvider,
    ILogger<ProcessTrackingHeaderMiddleware> _logger
)
{
    public async Task InvokeAsync(
        HttpContext httpContext,
        IProcessTrackingHeadersProvider processTrackingHeadersProvider,
        IProcessTrackingHandler processTrackingHandler
    )
    {
        var processTrackingDetail = ((IProcessTrackingHeadersProviderInternal)processTrackingHeadersProvider).GetProcessTrackingDetailFromHeaders(
            httpContext.Request
        );
        processTrackingHandler.SetProcessTrackingDetail(processTrackingDetail);

        using var scope = _logger.BeginScope(
            processTrackingDetail.GetActivityTags()
                .Concat(serviceMetadataProvider.GetMetadata().GetCustomDimensions())
        );

        await _next(httpContext);
    }
}