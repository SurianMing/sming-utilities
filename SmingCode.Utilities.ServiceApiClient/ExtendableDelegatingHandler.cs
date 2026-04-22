using Microsoft.Extensions.Logging;

namespace SmingCode.Utilities.ServiceApiClient;

internal class ExtendableDelegatingHandler<TService>(
    IEnumerable<IDelegateHandlingExtension>? _delegateHandlingExtensions,
    ApiClientConfiguration<TService> _apiClientConfiguration,
    ILogger<ExtendableDelegatingHandler<TService>> _logger
) : DelegatingHandler
    where TService : class
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            { "SmingProcessType", "ServiceApiClient" },
            { "ApiClient_TargetServiceName", _apiClientConfiguration.ServiceDisplayName }
        }))
        {
            foreach (var delegateHandlingExtension in _delegateHandlingExtensions ?? [])
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace(
                        "Running delegate handling extension {DelegateHandlingExtensionName}",
                        delegateHandlingExtension.GetType().Name
                    );
                }
                request = delegateHandlingExtension.Handle(request);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
