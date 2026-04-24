using Microsoft.Extensions.DependencyInjection;

namespace SmingCode.Utilities.ProcessTracking.WebApi;
using ServiceApiClient;
using StartupProcesses;

public static class Injection
{
    public static IValidProcessTrackingBuilder AddApiMiddleware(
        this IProcessTrackingBuilder builder
    )
    {
        var services = ((IProcessTrackingBuilderInternal)builder).Services;
        services.AddScoped<IServiceInitializer, ProcessTrackingInitialization>();
        services.AddScoped<IApiClientSendMiddleware, ProcessTrackingApiClientSendMiddleware>();

        return new ValidProcessTrackingBuilder(services);
    }
}
