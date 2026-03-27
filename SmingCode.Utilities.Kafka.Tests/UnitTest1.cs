using Xunit.Sdk;

namespace SmingCode.Utilities.Kafka.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        Delegate d = async (
            [TestAn] string stringParam,
            int intParam
        ) =>
        {
            return await Task.FromResult($"{stringParam}_{intParam}");
        };
        var d3 = new DelegateTester(
            async (
                string stringParam,
                int intParam
            ) =>
            {
                return await Task.FromResult($"{stringParam}_{intParam}");
            }
        );
        var d2 = d.GetType().GetGenericArguments().Last().Implements(typeof(IAsyncResult));

        var funcCall = (Func<string, int, Task<string>>)d;

        var result = await funcCall("TestString", 5);

        Assert.Equal("TestString_5", result);
    }

    internal class DelegateTester
    {
        internal DelegateTester(
            Delegate @delegate
        )
        {
            var d = @delegate.GetType();

            @delegate.DynamicInvoke("Hello", 2);
        }
    }

    // [Fact]
    // public async Task Test2()
    // {
    //     Delegate d = async (
    //         string stringParam,
    //         int intParam
    //     ) =>
    //     {
    //         return await Task.FromResult($"{stringParam}_{intParam}");
    //     };
    //     var d2 = d.GetType().GetGenericArguments().Last().Implements(typeof(IAsyncResult));

    //     var funcCall = (Func<string, int, Task<string>>)d;

    //     var testStuffInstance = new TestStuff<string, int, Task<string>>(
    //         funcCall,
    //         (str, i) => $"{str}__{i}",
    //         (str, i) => i + 1
    //     );

    //     var result = testStuffInstance.GetStuff("stringValue", 154);

    //     Assert.Equal("TestString_5", await result);
    // }
}

[AttributeUsage(AttributeTargets.Parameter)]
public class TestAnAttribute : Attribute
{
    
}
