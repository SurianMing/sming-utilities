using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmingCode.Utilities.StartupProcesses;

namespace SmingCode.Utilities.ServiceApiClient.Config;

public static class Configuration
{
    private const int DEFAULT_TIMEOUT_SECONDS = 60;

    public static IServiceCollection AddApiClient<TService>(
        this IServiceCollection services,
        string targetServiceDisplayName,
        string targetServiceName
    ) where TService : class
      => AddApiClient<TService>(
        services,
        targetServiceDisplayName,
        targetServiceName,
        _ => {},
        _ => {}
      );

    public static IServiceCollection AddApiClient<TService>(
        this IServiceCollection services,
        string targetServiceDisplayName,
        string targetServiceName,
        Action<HttpClient> clientConfiguration
    ) where TService : class
      => AddApiClient<TService>(
        services,
        targetServiceDisplayName,
        targetServiceName,
        clientConfiguration,
        _ => {}
      );

    public static IServiceCollection AddApiClient<TService>(
        this IServiceCollection services,
        string targetServiceDisplayName,
        string targetServiceName,
        Action<HttpClient> httpClientConfiguration,
        Action<IServiceApiClientConfigurationBuilder<TService>> apiClientConfiguration
    ) where TService : class
    {
        services.AddSingleton<MiddlewareHandler<TService>>();
        services.AddScoped<IServiceInitializer, ServiceApiClientInitialization<TService>>();

        services.AddHttpClient<IServiceApiClient<TService>, ApiClient<TService>>(config =>
        {
            config.BaseAddress = new Uri($"http://{targetServiceName}/");
            config.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT_SECONDS);
            httpClientConfiguration(config);
        });
        services.AddScoped<TService>();
        services.TryAddScoped(typeof(ApiClientMessageSender<,>));

        var apiClientConfigurationBuilder = new ServiceApiClientConfigurationBuilder<TService>(
            targetServiceDisplayName,
            targetServiceName,
            services
        );
        apiClientConfiguration(apiClientConfigurationBuilder);

        ApiClientDetail<TService> apiClientDetail = apiClientConfigurationBuilder.BuildApiClientDetail();
        services.AddSingleton(apiClientDetail);

        return services;
    }

    public static IServiceCollection AddApiClientMiddleware<TImplementation>(
        this IServiceCollection services
    )
    {
        services.AddSingleton(new MiddlewareDetail(typeof(TImplementation)));

        return services;
    }
}
