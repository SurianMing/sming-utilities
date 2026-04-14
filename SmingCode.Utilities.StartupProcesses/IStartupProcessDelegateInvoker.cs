using Microsoft.Extensions.Hosting;

namespace SmingCode.Utilities.StartupProcesses;

public interface IStartupProcessDelegateInvoker
{
    Task<bool> TryInvoke(
        IHost host,
        IServiceProvider serviceProvider,
        Delegate @delegate
    );
}
