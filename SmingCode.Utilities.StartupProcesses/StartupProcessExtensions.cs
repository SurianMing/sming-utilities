using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SmingCode.Utilities.StartupProcesses;

public static class StartupProcessExtensions
{
    public static async Task<IHost> RunUserDefinedStartupProcesses(
        this IHost host
    )
    {
        using var scope = host.Services.CreateScope();

        var serviceProvider = scope.ServiceProvider;
        var delegateInvokers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type.IsAssignableTo(typeof(IStartupProcessDelegateInvoker))
                && !type.IsAbstract && !type.IsInterface
            )
            .Select(Activator.CreateInstance)
            .OfType<IStartupProcessDelegateInvoker>()
            .ToList();

        var serviceInitializers = serviceProvider.GetService<IEnumerable<IServiceInitializer>>();

        if (serviceInitializers is not null)
        {
            foreach (var serviceInitializer in serviceInitializers)
            {
                var success = false;

                foreach (var delegateInvoker in delegateInvokers)
                {
                    success = success || await delegateInvoker.TryInvoke(
                        host,
                        serviceProvider,
                        serviceInitializer.ServiceInitializer
                    );
                }

                if (!success)
                {
                    throw new Exception("Unable to run all service initializers. Some startup processes may require additional startup processors to be loaded.");
                }
            }
        }

        return host;
    }
}
