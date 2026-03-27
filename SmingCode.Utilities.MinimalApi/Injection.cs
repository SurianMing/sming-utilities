using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SmingCode.Utilities.MinimalApi;

public static class Injection
{
    public static WebApplication MapEndpoints(
        this WebApplication app
    ) => app.MapEndpoints(Assembly.GetCallingAssembly());

    public static WebApplication MapEndpoints<TEndpointLocator>(
        this WebApplication app
    ) => app.MapEndpoints(
        Assembly.GetAssembly(typeof(TEndpointLocator))
            ?? throw new InvalidOperationException(
                $"No loaded assembly contains the Endpoint {typeof(TEndpointLocator).Name}."
            ));

    public static WebApplication MapEndpoints(
        this WebApplication app,
        Assembly endpointsAssembly
    )
    {
        var endpointImplementations = endpointsAssembly
            .GetTypes()
            .Where(type =>
                typeof(IMinimalEndpoint).IsAssignableFrom(type)
                && type is { IsInterface: false, IsAbstract: false }
            )
            .Select(type => ActivatorUtilities.CreateInstance(app.Services, type))
            .OfType<IMinimalEndpoint>()
            .ToList();

        endpointImplementations.ForEach(endpoint =>
            endpoint.MapEndpoint(app)
        );

        return app;
    }
}