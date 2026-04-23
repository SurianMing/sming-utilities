using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SmingCode.Utilities.ServiceApiClient;

public static class Injection
{
    private const int DEFAULT_TIMEOUT_SECONDS = 60;

    public static IServiceCollection AddApiClient<TInterface, TService>(
        this IServiceCollection services,
        string targetServiceDisplayName,
        string targetServiceName
    ) where TInterface : class
      where TService : class, TInterface
    {
        services.AddTransient<ExtendableDelegatingHandler<TService>>();
        ApiClientConfiguration<TService> apiClientConfiguration = new(
            targetServiceDisplayName,
            targetServiceName
        );
        services.AddSingleton(apiClientConfiguration);

        services.AddHttpClient<TInterface, TService>(config =>
        {
            config.BaseAddress = new Uri($"http://{targetServiceName}");
            config.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT_SECONDS);
        }).AddHttpMessageHandler<ExtendableDelegatingHandler<TService>>();

        return services;
    }

    public static IServiceCollection AddApiClient<TInterface, TService>(
        this IServiceCollection services,
        string targetServiceDisplayName,
        string targetServiceName,
        Action<HttpClient> clientConfiguration
    ) where TInterface : class
      where TService : class, TInterface
    {
        services.AddTransient<ExtendableDelegatingHandler<TService>>();
        ApiClientConfiguration<TService> apiClientConfiguration = new(
            targetServiceDisplayName,
            targetServiceName
        );
        services.AddSingleton(apiClientConfiguration);

        services.AddHttpClient<TInterface, TService>(config =>
        {
            config.BaseAddress = new Uri($"http://{targetServiceName}");
            config.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT_SECONDS);
            clientConfiguration(config);
        }).AddHttpMessageHandler<ExtendableDelegatingHandler<TService>>();

        return services;
    }
}
