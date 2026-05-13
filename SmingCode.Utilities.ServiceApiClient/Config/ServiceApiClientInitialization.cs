using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SmingCode.Utilities.ServiceApiClient.Config;
using StartupProcesses;

internal class ServiceApiClientInitialization<TService> : IServiceInitializer
    where TService : class
{
    private static readonly Type _contextType = typeof(ApiClientSendContext);
    private static readonly PropertyInfo _contextServiceProviderProperty
        = typeof(ApiClientSendContext)
            .GetProperty(
                nameof(ApiClientSendContext.ServiceProvider),
                BindingFlags.NonPublic | BindingFlags.Instance
            )!;
    private static readonly MethodInfo _sendDelegateBuilder =
        typeof(ServiceApiClientInitialization<TService>)
            .GetMethod(
                "BuildApiClientSendDelegate",
                BindingFlags.Static | BindingFlags.NonPublic
            )!;
    private static readonly MethodInfo _getRequiredServiceMethod =
        typeof(ServiceProviderServiceExtensions)
            .GetMethods(
                BindingFlags.Static | BindingFlags.Public
            )
            .Single(method =>
                method.Name == "GetRequiredService"
                && method.IsGenericMethod
            );
    private static readonly MethodInfo _getRequiredKeyedServiceMethod =
        typeof(ServiceProviderKeyedServiceExtensions)
            .GetMethods(
                BindingFlags.Static | BindingFlags.Public
            )
            .Single(method =>
                method.Name == "GetRequiredKeyedService"
                && method.IsGenericMethod
            );

    public Delegate ServiceInitializer =>
        (
            IServiceProvider serviceProvider,
            MiddlewareHandler<TService> middlewareHandler,
            ApiClientDetail<TService> _apiClientDetail,
            IEnumerable<MiddlewareDetail>? middlewareDetails
        ) =>
        {
            SendDelegate nextPipelineEntryDelegate = async context => await context.MessageSender(context);

            if (middlewareDetails is not null)
            {
                var middlewareToInclude = middlewareDetails
                    .Where(detail => detail.ServiceType is null || detail.ServiceType == typeof(TService))
                    .Reverse();

                foreach (var middleware in middlewareToInclude)
                {
                    var buildApiDelegateMethod = _sendDelegateBuilder
                        !.MakeGenericMethod(middleware.MiddlewareImplementation);

                    var delegateMethod = (SendDelegate)buildApiDelegateMethod.Invoke(null, [ nextPipelineEntryDelegate, _apiClientDetail, serviceProvider ])!;

                    nextPipelineEntryDelegate = delegateMethod;
                }
            }

            middlewareHandler.SetMessageSender(nextPipelineEntryDelegate);
        };

    private static SendDelegate BuildApiClientSendDelegate<T>(
        SendDelegate nextPipelineEntryDelegate,
        ApiClientDetail<TService> apiClientDetail,
        IServiceProvider serviceProvider
    )
    {
        var middlewareType = typeof(T);

        var constructor = typeof(T).GetConstructors(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
        ).Single();
        var constructorParameters = constructor.GetParameters()
            .Select(parameter => parameter.ParameterType)
            .Select(parameterType =>
                parameterType == typeof(SendDelegate)
                    ? nextPipelineEntryDelegate
                    : apiClientDetail.ClientSpecificRegisteredTypes.Contains(parameterType)
                        ? serviceProvider.GetRequiredKeyedService(parameterType, apiClientDetail.ClientSpecificServiceKey)
                        : serviceProvider.GetRequiredService(parameterType)
            );
        var middlewareSingletonInstance = (T)constructor.Invoke([ ..constructorParameters ]);

        Expression[] parameterBuilderExpressions = [];
        var handleAsyncMethod = middlewareType.GetMethod("HandleAsync");
        if (handleAsyncMethod is null || handleAsyncMethod.ReturnType != typeof(Task))
        {
            throw new InvalidOperationException(
                $"Attempt to inject ServiceApiClient middleware {middlewareType.Name} failed as it has no HandleAsync method with return type Task"
            );
        }

        var handleAsyncMethodParameters = handleAsyncMethod.GetParameters();
        var contextParameters = handleAsyncMethodParameters.Where(param =>
            param.ParameterType == _contextType
            && param.Name == "context"
        ).ToArray();
        if (contextParameters.Length != 1)
        {
            throw new InvalidOperationException(
                $"Attempt to inject ServiceApiClient middleware {middlewareType.Name} failed as it's HandleAsync must have exactly one parameter of type {nameof(ApiClientSendContext)} with name 'context'."
            );
        }

        var instanceParameter = Expression.Parameter(middlewareType, "instance");
        var contextParameter = Expression.Parameter(_contextType, "context");
        var serviceProviderProperty = Expression.Property(contextParameter, _contextServiceProviderProperty);
        var apiClientKey = Expression.Constant(apiClientDetail.ClientSpecificServiceKey);

        foreach (var handleAsyncParameter in handleAsyncMethodParameters)
        {
            var parameterType = handleAsyncParameter.ParameterType;
            var parameterName = handleAsyncParameter.Name;

            parameterBuilderExpressions = [
                .. parameterBuilderExpressions,
                parameterType == _contextType
                    ? contextParameter
                    : apiClientDetail.ClientSpecificRegisteredTypes.Contains(typeof(T))
                        ? Expression.Call(null, _getRequiredKeyedServiceMethod.MakeGenericMethod(parameterType), serviceProviderProperty, apiClientKey)
                        : Expression.Call(null, _getRequiredServiceMethod.MakeGenericMethod(parameterType), serviceProviderProperty)
            ];
        }
        var methodCall = Expression.Call(instanceParameter, handleAsyncMethod, parameterBuilderExpressions);
        var method = Expression.Lambda<Func<T, ApiClientSendContext, Task>>(methodCall, instanceParameter, contextParameter).Compile();

        return async context => await method(middlewareSingletonInstance, context);
    }
}