using System.Net.Mime;
using Microsoft.Extensions.DependencyInjection;

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
        ApiClientConfiguration<TService> apiClientConfiguration = new(
            targetServiceDisplayName,
            targetServiceName
        );
        services.AddSingleton(apiClientConfiguration);

        services.AddHttpClient<IServiceApiClient<TService>, ApiClient<TService>>(config =>
        {
            config.BaseAddress = new Uri($"http://{targetServiceName}");
            config.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT_SECONDS);
        });
        services.AddScoped<TInterface, TService>();

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
        ApiClientConfiguration<TService> apiClientConfiguration = new(
            targetServiceDisplayName,
            targetServiceName
        );
        services.AddSingleton(apiClientConfiguration);

        services.AddHttpClient<IServiceApiClient<TService>, ApiClient<TService>>(config =>
        {
            config.BaseAddress = new Uri($"http://{targetServiceName}");
            config.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT_SECONDS);
            clientConfiguration(config);
        });
        services.AddScoped<TInterface, TService>();

        return services;
    }
}
