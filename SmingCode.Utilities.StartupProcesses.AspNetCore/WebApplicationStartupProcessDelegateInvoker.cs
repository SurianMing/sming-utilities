using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace SmingCode.Utilities.StartupProcesses.AspNetCore;

internal class WebApplicationStartupProcessDelegateInvoker : IStartupProcessDelegateInvoker
{
    public async Task<bool> TryInvoke(
        IHost host,
        IServiceProvider serviceProvider,
        Delegate @delegate
    )
    {
        if (host is not WebApplication webApplication)
        {
            return false;
        }

        var delegateParams = @delegate.Method.GetParameters()
            .Select(parameterInfo =>
                parameterInfo.ParameterType.IsAssignableFrom(typeof(WebApplication))
                    ? webApplication
                    : serviceProvider.GetService(parameterInfo.ParameterType)
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