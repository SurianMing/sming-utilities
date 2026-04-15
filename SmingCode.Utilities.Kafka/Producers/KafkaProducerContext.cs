using System.Text;

namespace SmingCode.Utilities.Kafka.Producers;

public interface IKafkaProducerContext<TKey, TValue>
{
    string Topic { get; }
    TKey Key { get; }
    TValue Value { get; }
    Headers Headers { get; }
    IServiceProvider ServiceProvider { get; }
    IKafkaProducerContext<TKey, TValue> AddHeader(
        string headerName,
        string value
    );
    bool HasKey { get; }
}

internal class KafkaProducerContext<TKey, TValue>(
    string topic,
    TKey? key,
    TValue value,
    Headers headers,
    IServiceProvider serviceProvider
) : IKafkaProducerContext<TKey, TValue>
{
    private readonly TKey? _key =
        key is not null || typeof(TKey) == typeof(Ignore)
            ? key
            : throw new Exception();

    public string Topic { get; private set; } = topic;
    public TKey Key => _key
        ?? throw new InvalidOperationException("Attempt to retrieve the Key value when it has not been set. Please check HasKey first.");
    public TValue Value { get; private set; } = value;
    public Headers Headers { get; private set; } = headers;
    public IServiceProvider ServiceProvider { get; private set; } = serviceProvider;

    public IKafkaProducerContext<TKey, TValue> AddHeader(
        string headerName,
        string value
    )
    {
        Headers.Add(new Header(
            headerName,
            Encoding.UTF8.GetBytes(value)
        ));

        return this;
    }

    public bool HasKey => Key is null;
}
