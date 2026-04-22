using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SmingCode.Utilities.ServiceApiClient;

public static class Injection
{
    public static IServiceCollection AddApiClient<TInterface, TService>(
        this IServiceCollection services,
        string serviceDisplayName,
        string serviceName
    ) where TInterface : class
      where TService : class, TInterface
    {
        services.AddTransient<ExtendableDelegatingHandler<TService>>();
        ApiClientConfiguration<TService> apiClientConfiguration = new(
            serviceDisplayName,
            serviceName
        );
        services.AddSingleton(apiClientConfiguration);

        services.AddHttpClient<TService>(config =>
        {
            config.BaseAddress = new Uri($"http://{serviceName}");
        }).AddHttpMessageHandler<ExtendableDelegatingHandler<TService>>();

        return services;
    }
}

internal record ApiClientConfiguration<TService>(
    string ServiceDisplayName,
    string ServiceName
);

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

public interface IDelegateHandlingExtension
{
    HttpRequestMessage Handle(
        HttpRequestMessage requestMessage
    );
}