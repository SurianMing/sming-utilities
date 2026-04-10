using Microsoft.Extensions.DependencyInjection;

namespace SmingCode.Utilities.ServiceMetadata;

public static class Injection
{
    public static IServiceCollection InitializeServiceMetadata(
        this IServiceCollection services
    ) => services.AddSingleton<IServiceMetadataProvider, ServiceMetadataProvider>();
}
