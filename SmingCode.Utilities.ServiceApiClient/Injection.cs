using Microsoft.Extensions.DependencyInjection;

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
