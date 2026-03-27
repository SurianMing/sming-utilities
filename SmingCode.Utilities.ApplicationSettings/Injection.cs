using Microsoft.Extensions.DependencyInjection;

namespace SmingCode.Utilities.ApplicationSettings;

public static class Injection
{
    public static IServiceCollection InitialiseApplicationSettings(
        this IServiceCollection services
    )
    {
        services.AddOptions<SmingApplicationSettings>()
            .BindConfiguration("SmingApplication");

        return services;
    }
}