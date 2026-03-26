using System.Reflection;
using System.Text.Json;
using SurianMing.Utilities.DelegateInvokers;

namespace SurianMing.Utilities.Kafka;

internal class KafkaDelegateInvoker<TKey, TValue>
{
    private readonly IDelegateInvoker<IServiceProvider, ConsumeResult<TKey, TValue>, KafkaEventResult> _invoker;

    internal class ParameterBuilderBuilder : DelegateParameterBuilderBuilder<IServiceProvider, ConsumeResult<TKey, TValue>>
    {
        public override Func<IServiceProvider, ConsumeResult<TKey, TValue>, TParam> BuildParameterBuilder<TParam>(
            ParameterInfo parameterInfo
        ) => parameterInfo.GetCustomAttribute<FromEventKeyAttribute>() is not null
                ? typeof(TKey) == typeof(TParam)
                    ? (_, consumeResult) => consumeResult.Message.Key is TParam tParamVal
                        ? tParamVal
                        : throw new InvalidCastException("Mismatched key type in kafka message handling")
                    : (_, consumeResult) => JsonSerializer.Deserialize<TParam>(consumeResult.Message.Key!.ToString()!)!
                : parameterInfo.GetCustomAttribute<FromEventValueAttribute>() is not null
                    ? typeof(TValue) == typeof(TParam)
                        ? (_, consumeResult) => consumeResult.Message.Value is TParam tParamVal
                            ? tParamVal
                            : throw new InvalidCastException("Mismatched value type in kafka message handling")
                        // (TParam)Convert.ChangeType(consumeResult.Message.Value, typeof(TParam))!
                        : (_, consumeResult) => JsonSerializer.Deserialize<TParam>(consumeResult.Message.Value!.ToString()!)!
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