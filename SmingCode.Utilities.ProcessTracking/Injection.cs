using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace SmingCode.Utilities.ProcessTracking;

public static class Injection
{
    public static IServiceCollection AddProcessTracking(
        this IServiceCollection services,
        Func<IProcessTrackingBuilder, IValidProcessTrackingBuilder> initialization
    )
    {
        var processTrackingBuilder = new ProcessTrackingBuilder(services);
        initialization(processTrackingBuilder);

        services.AddScoped<IProcessTrackingHandler, ProcessTrackingHandler>();
        // services.AddSingleton<IProcessTrackingManager, ProcessTrackingManager>();
        // services.AddSingleton<ProcessTrackingHandlerManager>();

        return services;
    }
}
