using Microsoft.Extensions.Hosting;

namespace SmingCode.Utilities.StartupProcesses.AspNetCore;

internal class DefaultStartupProcessDelegateInvoker : IStartupProcessDelegateInvoker
{
    public async Task<bool> TryInvoke(
        IHost host,
        IServiceProvider serviceProvider,
        Delegate @delegate
    )
    {
        var delegateParams = @delegate.Method.GetParameters()
            .Select(parameterInfo =>
                serviceProvider.GetService(parameterInfo.ParameterType)
            ).ToArray();

        if (delegateParams.Any(param => param is null))
        {
            return false;
        }

        if (@delegate.GetType().GetGenericArguments().Last().IsAssignableFrom(typeof(Task<>)))
        {
            var result = @delegate.DynamicInvoke(delegateParams);

            await (Task)result!;
        }
        else
        {
            @delegate.DynamicInvoke(delegateParams);
        }

        return true;
    }
}