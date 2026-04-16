using System.Reflection;

namespace SmingCode.Utilities.Kafka.Host.Consumers;
using DelegateInvokers;

internal class KafkaDelegateInvoker<TKey, TValue>
{
    private readonly IDelegateInvoker<IServiceProvider, ConsumeResult<TKey, TValue>, KafkaEventResult> _invoker;

    internal class ParameterBuilderBuilder : DelegateParameterBuilderBuilder<IServiceProvider, ConsumeResult<TKey, TValue>>
    {
        public override Func<IServiceProvider, ConsumeResult<TKey, TValue>, TParam> BuildParameterBuilder<TParam>(
            ParameterInfo parameterInfo
        ) => parameterInfo.GetCustomAttribute<FromEventKeyAttribute>() is not null
                ? (_, consumeResult) => consumeResult.Message.Key is TParam tParamVal
                    ? tParamVal
                    : throw new InvalidCastException("Mismatched key type in kafka message handling")
                : parameterInfo.GetCustomAttribute<FromEventValueAttribute>() is not null
                    ? (_, consumeResult) => consumeResult.Message.Value is TParam tParamVal
                        ? tParamVal
                        : throw new InvalidCastException("Mismatched value type in kafka message handling")
                    : (serviceProvider, _) => serviceProvider.GetService<TParam>()!;
    }

    internal KafkaDelegateInvoker(
        Delegate @delegate
    ) => _invoker = DelegateInvoker<IServiceProvider, ConsumeResult<TKey, TValue>, KafkaEventResult>.FromDelegate(
        @delegate,
        new ParameterBuilderBuilder()
    );

    public async Task<KafkaEventResult> Invoke(
        IServiceProvider serviceProvider,
        ConsumeResult<TKey, TValue> consumeResult
    ) => await _invoker.Invoke(serviceProvider, consumeResult);
}