using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}
