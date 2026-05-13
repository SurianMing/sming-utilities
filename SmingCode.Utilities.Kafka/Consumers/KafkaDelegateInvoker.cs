using System.Reflection;

namespace SmingCode.Utilities.Kafka.Consumers;
using DelegateInvokers;

internal class KafkaDelegateInvoker<TKey, TValue>
{
    private readonly IDelegateInvoker<IServiceProvider, TKey?, TValue?, KafkaEventResult> _invoker;

    internal class ParameterBuilderBuilder : DelegateParameterBuilderBuilder<IServiceProvider, TKey?, TValue?>
    {
        public override Func<IServiceProvider, TKey?, TValue?, TParam> BuildParameterBuilder<TParam>(
            ParameterInfo parameterInfo
        ) => parameterInfo.GetCustomAttribute<FromEventKeyAttribute>() is not null
                ? (_, key, _) => key is not null && key is TParam tParamVal
                    ? tParamVal
                    : throw new InvalidCastException("Mismatched key type in kafka message handling")
                : parameterInfo.GetCustomAttribute<FromEventValueAttribute>() is not null
                    ? (_, _, value) => value is not null && value is TParam tParamVal
                        ? tParamVal
                        : throw new InvalidCastException("Mismatched value type in kafka message handling")
                    : (serviceProvider, _, _) => serviceProvider.GetService<TParam>()!;
    }

    internal KafkaDelegateInvoker(
        Delegate @delegate
    ) => _invoker = DelegateInvoker<IServiceProvider, TKey?, TValue?, KafkaEventResult>.FromDelegate(
        @delegate,
        new ParameterBuilderBuilder()
    );

    public async Task<KafkaEventResult> Invoke(
        IServiceProvider serviceProvider,
        TKey? key,
        TValue? value
    ) => await _invoker.Invoke(serviceProvider, key, value);
}