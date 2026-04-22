namespace SmingCode.Utilities.StartupProcesses;

internal class StartupProcessDelegateInvokerProvider : IStartupProcessDelegateInvokerProvider, IStartupProcessDependency
{
    private IStartupProcessDelegateInvoker[] _delegateInvokers = [ new DefaultStartupProcessDelegateInvoker() ];

    public IStartupProcessDependency AddStartupProcessDelegateInvoker(
        IStartupProcessDelegateInvoker newStartupProcessDelegateInvoker
    )
    {
        _delegateInvokers = [
            .. _delegateInvokers,
            newStartupProcessDelegateInvoker
        ];

        return this;
    }

    public IStartupProcessDelegateInvoker[] GetStartupProcessDelegateInvokers()
        => _delegateInvokers;
}
