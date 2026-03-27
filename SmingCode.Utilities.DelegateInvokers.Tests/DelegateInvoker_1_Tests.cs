using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace SmingCode.Utilities.DelegateInvokers.Tests;

public class DelegateInvoker_1_Tests
{
    [Fact]
    public async Task Test1()
    {
        // Arrange
        var test = DelegateInvoker<IServiceProvider, string>.FromDelegate(
            async (
                [FromServices] ITestClass testClass,
                int testInt
            ) =>
            {
                return await Task.FromResult($"Result_{testClass.TestProp}_{testInt}");
            },
            new TestDelegateParameterBuilderBuilder()
        );
        var serviceProvider = new ServiceCollection()
            .AddScoped<ITestClass, TestClass>()
            .BuildServiceProvider();
        var expectedResult = "Result_TestPropValue_0";

        // Act
        var result = await test.Invoke(serviceProvider);

        // Assert
        result.ShouldBe(expectedResult);
    }
}

[AttributeUsage(AttributeTargets.Parameter)]
internal class FromServicesAttribute : Attribute
{ }

internal interface ITestClass
{
    public string TestProp { get; }
}

internal class TestClass : ITestClass
{
    public string TestProp => "TestPropValue";
}

internal class TestDelegateParameterBuilderBuilder : DelegateParameterBuilderBuilder<IServiceProvider>
{
    public override Func<IServiceProvider, TParam> BuildParameterBuilder<TParam>(ParameterInfo parameterInfo)
        => parameterInfo.GetCustomAttribute<FromServicesAttribute>() is not null
            ? serviceProvider => serviceProvider.GetService<TParam>()!
            : _ => default(TParam)!;
}