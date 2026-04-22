using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SmingCode.Utilities.StartupProcesses;

public static class StartupProcessExtensions
{
    public static async Task<IHost> RunUserDefinedStartupProcesses(
        this IHost host,
        Action<IStartupProcessDependency>? dependencyManager = null
    )
    {
        using var scope = host.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var serviceInitializers = serviceProvider.GetService<IEnumerable<IServiceInitializer>>()
            ?.ToArray();

        if (serviceInitializers is null || serviceInitializers.Length == 0)
        {
            return host;
        }

        var startProcessInvokersProvider = new StartupProcessDelegateInvokerProvider();
        if (dependencyManager is not null)
        {
            dependencyManager(startProcessInvokersProvider);
        }
        var delegateInvokers = startProcessInvokersProvider.GetStartupProcessDelegateInvokers();

        foreach (var serviceInitializer in serviceInitializers)
        {
            var success = await TryRunningServiceInitializer(
                serviceInitializer,
                delegateInvokers,
                host,
                serviceProvider
            );

            if (!success)
            {
                throw new Exception("Unable to run all service initializers. Some startup processes may require additional startup processors to be loaded.");
            }
        }

        return host;
    }

    private static async Task<bool> TryRunningServiceInitializer(
        IServiceInitializer serviceInitializer,
        IStartupProcessDelegateInvoker[] delegateInvokers,
        IHost host,
        IServiceProvider serviceProvider
    )
    {
        foreach (var delegateInvoker in delegateInvokers)
        {
            if (await delegateInvoker.TryInvoke(
                host,
                serviceProvider,
                serviceInitializer.ServiceInitializer
            ))
            {
                return true;
            }
        }

        return false;
    }
}
