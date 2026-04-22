using Microsoft.Extensions.DependencyInjection;

namespace SmingCode.Utilities.ProcessTracking.WebApi;

using SmingCode.Utilities.ServiceApiClient;
using StartupProcesses;

public static class Injection
{
    public static IValidProcessTrackingBuilder AddApiMiddleware(
        this IProcessTrackingBuilder builder
    )
    {
        var services = ((IProcessTrackingBuilderInternal)builder).Services;
        services.AddScoped<IServiceInitializer, ProcessTrackingInitialization>();
        services.AddScoped<IDelegateHandlingExtension, ProcessTrackingHttpClientInjection>();

        return new ValidProcessTrackingBuilder(services);
    }
}
