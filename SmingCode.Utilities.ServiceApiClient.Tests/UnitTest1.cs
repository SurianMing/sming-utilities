using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SmingCode.Utilities.ServiceApiClient.Tests;

public class UnitTest1
{
    private static MethodInfo _getRequiredServiceMethod = typeof(ServiceProviderServiceExtensions)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(
            method => method.Name == "GetRequiredService"
            && method.IsGenericMethodDefinition
        );

    [Fact]
    public void Test1()
    {
        Delegate d = async (
            IServiceApiClient<UnitTest1> stringParam,
            ApiClientSendContext<ApiClientConfiguration<NoResponse>, NoResponse> context
        ) =>
        {
            return await Task.FromResult("Hello");
        };
        var serviceProvider = new ServiceCollection()
            .AddSingleton(new MiddlewareEntry(typeof(FinalHandler)))
            .AddSingleton(new MiddlewareEntry(typeof(MiddlewareTest<,>)))
            .AddScoped<ITestInterface2, TestClass>()
            .BuildServiceProvider();

        // var handleMethod = middleware.GetType().GetMethod("HandleAsync")
        //     ?? throw new InvalidOperationException(
        //         "Attempt to inject ApiClientSend middleware, but injected class does not contain a HandleAsync method."
        //     );

        var finalHandler = new FinalHandler();
        var delegateType = typeof(ApiClientSendDelegate<,>);
        ApiClientSendDelegate<string, Task<string>> finalMethodCall = async context =>
            await finalHandler.HandleAsync(context);

        var nextHandlerToCall = (typeof(FinalHandler), typeof(FinalHandler).GetMethod("HandleAsync"), Array.Empty<ParameterInfo>());

        foreach (var middleware in serviceProvider.GetRequiredService<IEnumerable<MiddlewareEntry>>())
        {
            object[] instanceConstructorParams = [];


            foreach (var constructorParamInfo in nextHandlerToCall.Item3)
            {
                instanceConstructorParams = [
                    .. instanceConstructorParams,
                    constructorParamInfo.ParameterType == 
                ]
            }
        }

        var contextMethodType = typeof(ApiClientSendContext<ApiClientConfiguration<NoResponse>, NoResponse>);
        var method = d.Method;
        var handleMethodParams = method.GetParameters();


        if (handleMethodParams.All(
            param => param.ParameterType == contextMethodType
                && param.Name == "context"))
        {
            throw new InvalidOperationException(
                $"Attempt to inject ApiClientSend middleware, but injected class's HandleAsync method does not take a context parameter of type {contextMethodType.Name}."
            );
        }

        
        var contextParam = Expression.Parameter(contextMethodType, "context");

        ParameterExpression[] parameterExpressions = [
            .. handleMethodParams
                .Select(param => Expression.Parameter(param.ParameterType, param.Name))
        ];
        // var methodCall = Expression.Call()

        // var test2 = Expression.Lambda()
        // foreach (var param in handleMethodParams)
        // {
        //     var newParameter = param.ParameterType == contextMethodType
        //         ? Expression.Constant(contextParam)
        //         : Expression.Parameter(
        //             param.ParameterType,
        //             Expression.(
        //                 _getRequiredServiceMethod.MakeGenericMethod(param.ParameterType),
        //                 serviceProvider
        //             )
        //         );
        //     //     ? contextParam
        //     //     : Expression.Parameter(param.ParameterType, Expression.Constant(Expression.Call()))
        // }
    }
}

public record MiddlewareEntry(
    Type ThisEntry
);

internal class FinalHandler
{
    public async Task<TResponse> HandleAsync<TBody, TResponse>(
        ApiClientSendContext<TBody, TResponse> context
    )
    {
        return default!;
    }
}

internal delegate Task<TResponse> ApiClientSendDelegate<TBody, TResponse>(
    ApiClientSendContext<TBody, TResponse> context
);

internal class MiddlewareTest<TBody, TResponse>(
    ApiClientSendDelegate<TBody, TResponse> _sendDelegate,
    ILogger<MiddlewareTest<TBody, TResponse>> _logger
)
{
    public async Task<TResponse> HandleAsync(
        ApiClientSendContext<TBody, TResponse> context,
        ITestInterface2 testInterface2
    )
    {
        return await _sendDelegate(context);
    }
}

internal interface ITestInterface2 { }
internal class TestClass : ITestInterface2
{
    
}