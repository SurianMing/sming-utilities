namespace SmingCode.Utilities.Kafka.Producers;

internal class KafkaProducer(
    KafkaServerOptions _kafkaServerOptions,
    IEnumerable<IKafkaProducerMiddleware> _kafkaProducerMiddlewares,
    IServiceProvider _serviceProvider
) : IKafkaProducer
{
    public async Task<bool> SendEvent<TValue>(
        string topic,
        TValue value
    ) => await ProcessKafkaEvent<Ignore, TValue>(
        topic,
        null,
        value
    );

    public async Task<bool> SendEvent<TKey, TValue>(
        string topic,
        TKey key,
        TValue value
    ) => await ProcessKafkaEvent(
        topic,
        key,
        value
    );

    public async Task<bool> SendEvent<TValue>(
        string topic,
        TValue value,
        Headers headers
    ) => await ProcessKafkaEvent<Ignore, TValue>(
        topic,
        null,
        value,
        headers
    );

    public async Task<bool> SendEvent<TKey, TValue>(
        string topic,
        TKey key,
        TValue value,
        Headers headers
    ) => await ProcessKafkaEvent(
        topic,
        key,
        value,
        headers
    );

    private async Task<bool> ProcessKafkaEvent<TKey, TValue>(
        string topic,
        TKey? key,
        TValue value,
        Headers? headers = null
    )
    {
        var kafkaProducerContext = new KafkaProducerContext<TKey, TValue>(
            topic,
            key,
            value,
            headers ?? [],
            _serviceProvider
        ).AddHeader("message-identifier", Guid.NewGuid().ToString());

        var produceDelegate = new KafkaProducerDelegate<TKey, TValue>(async (kafkaProducerContext) =>
        {
            using var producer = BuildProducer<TKey, TValue>();
            
            var message = new Message<TKey, TValue>
            {
                Value = kafkaProducerContext.Value,
                Headers = kafkaProducerContext.Headers
            };

            if (typeof(TKey) != typeof(Ignore) && kafkaProducerContext.HasKey)
            {
                message.Key = kafkaProducerContext.Key;
            }

            var deliveryResult = await producer.ProduceAsync(
                kafkaProducerContext.Topic,
                message
            );

            return deliveryResult.Status == PersistenceStatus.Persisted;
        });

        foreach (var kafkaProducerMiddleware in _kafkaProducerMiddlewares)
        {
            produceDelegate = new KafkaProducerDelegate<TKey, TValue>(async (kafkaProducerContext) =>
                await produceDelegate(kafkaProducerContext)
            );
        }

        var result = await produceDelegate(kafkaProducerContext);
        return result;
    }

    private IProducer<TKey, TValue> BuildProducer<TKey, TValue>()
    {
        var producerBuilder = new ProducerBuilder<TKey, TValue>(
            new ProducerConfig
            {
                BootstrapServers = _kafkaServerOptions.BootstrapServers,
                ApiVersionRequest = false,
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 1000,
                Acks = Acks.All
            });

        if (typeof(TKey) != typeof(Ignore))
        {
            producerBuilder.SetKeySerializer(KafkaSerializerFactory.GetSerializer<TKey>());
        }
        if (typeof(TValue) != typeof(Ignore))
        {
            producerBuilder.SetValueSerializer(KafkaSerializerFactory.GetSerializer<TValue>());
        }

        return producerBuilder.Build();
    }
}
