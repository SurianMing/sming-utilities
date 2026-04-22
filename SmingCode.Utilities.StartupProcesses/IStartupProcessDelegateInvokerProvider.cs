namespace SmingCode.Utilities.StartupProcesses;

internal interface IStartupProcessDelegateInvokerProvider : IStartupProcessDependency
{
    IStartupProcessDependency AddStartupProcessDelegateInvoker(
        IStartupProcessDelegateInvoker newStartupProcessDelegateInvoker
    );
    IStartupProcessDelegateInvoker[] GetStartupProcessDelegateInvokers();
}
