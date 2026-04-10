using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SmingCode.Utilities.ProcessTracking.WebApi;

public static class Injection
{
    public static IValidProcessTrackingBuilder AddApiMiddleware(
        this IProcessTrackingBuilder builder
    )
    {
        var services = ((IProcessTrackingBuilderInternal)builder).Services;
        services.AddScoped<IProcessTrackingHeadersProvider, ProcessTrackingHeadersProvider>();

        return new ValidProcessTrackingBuilder(services);
    }

    public static IApplicationBuilder UseProcessTrackingMiddleware(
        this IApplicationBuilder applicationBuilder
    )
    {
        applicationBuilder.UseMiddleware<ProcessTrackingHeaderMiddleware>();

        return applicationBuilder;
    }
}