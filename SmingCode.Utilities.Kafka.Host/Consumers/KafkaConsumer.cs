namespace SmingCode.Utilities.Kafka.Host.Consumers;
using ServiceMetadata;

internal class KafkaConsumer<TKey, TValue>(
    IServiceScopeFactory _serviceScopeFactory,
    ITopicManager _topicManager,
    KafkaConsumerDefinition<TKey, TValue> _kafkaConsumerDefinition,
    IServiceMetadataProvider serviceMetadataProvider,
    KafkaServerOptions _kafkaServerOptions,
    IEnumerable<IKafkaConsumerMiddleware> _kafkaConsumerMiddlewares,
    ILogger<KafkaConsumer<TKey, TValue>> _logger
) : IKafkaConsumer
{
    private readonly string _fullServiceDescriptor = serviceMetadataProvider.GetMetadata().FullServiceDescriptor;

    public void InitialiseEventConsumer(
        CancellationToken cancellationToken
    )
    {
        var topicToConsume = GetTopicToConsume();
        var clientGroupId = GetClientGroupId();
        var consumer = BuildConsumer(
            topicToConsume,
            clientGroupId
        );

        Task.Run(() => MetadataRefresh(consumer.Handle), cancellationToken);

        consumer.Subscribe(topicToConsume);

        var consumerTask = Task.Run(() =>
        {
            try
            {
                _logger.LogInformation(
                    "Starting consumer on topic {topicToConsume}.",
                    topicToConsume
                );

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var cr = consumer.Consume(TimeSpan.FromMilliseconds(1000));

                        if (cr is not null && cr.Topic != "__consumer_offsets")
                        {
                            var trackingId = Guid.NewGuid();
                            _logger.LogTrace("Message received from topic {topicToConsume}, processing...", topicToConsume);

                            Task.Run(async () =>
                            {
                                try
                                {
                                    using var scope = _serviceScopeFactory.CreateScope();
                                    var result = await ProcessKafkaEvent(
                                        topicToConsume,
                                        cr
                                    );

                                    if (result == KafkaEventResult.Complete)
                                    {
                                        _logger.LogInformation("Successfully consumed message.");

                                        consumer.StoreOffset(cr);
                                    }
                                    else
                                    {
                                        _logger.LogWarning(
                                            "Consumer failed to complete processing for message from topic {topicToConsume} with value {messageValue}.",
                                            topicToConsume, cr.Message.Value
                                        );
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Exception occurred whilst processing message.");
                                }
                            });
                        }
                    }
                    catch (ConsumeException e)
                    {
                        //We can get this when we consume from a queue not yet created.
                        //The first message sent to that queue will then create the message
                        if (!e.Message.Contains("Broker: Unknown topic or partition"))
                            _logger.LogWarning("Subscription to topic '{topicToConsume}' has raised an exception, but will continue until stopped.",
                                topicToConsume);
                    }
                    catch { }
                }
            }
            catch (OperationCanceledException)
            {
                // Close and Release all the resources held by this consumer
                _logger.LogError("Subscription to topic '{topicToConsume}' has been stopped.",
                    topicToConsume);
                consumer.Close();
                consumer.Dispose();
            }
        }, cancellationToken);
    }

    private async Task<KafkaEventResult> ProcessKafkaEvent(
        string topicConsumed,
        ConsumeResult<TKey, TValue> consumeResult
    )
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var kafkaConsumerContext = new KafkaConsumerContext<TKey, TValue>(
            topicConsumed,
            consumeResult,
            scope.ServiceProvider
        );

        var handlerDelegate = new KafkaConsumeDelegate<TKey, TValue>(async (kafkaConsumerContext) =>
            await _kafkaConsumerDefinition.Handler.Invoke(
                kafkaConsumerContext.ServiceProvider,
                kafkaConsumerContext.ConsumeResult
            )
        );

        foreach (var kafkaConsumerMiddleware in _kafkaConsumerMiddlewares)
        {
            handlerDelegate = new KafkaConsumeDelegate<TKey, TValue>(async (kafkaConsumerContext) =>
                await handlerDelegate(kafkaConsumerContext)
            );
        }

        var result = await handlerDelegate(kafkaConsumerContext);
        return result;
    }

    private string GetTopicToConsume()
        => _kafkaConsumerDefinition.UseRegexPatternMatching
            ? $"^{_kafkaConsumerDefinition.TopicToMatch}"
            : _kafkaConsumerDefinition.TopicToMatch;

    private string GetClientGroupId()
        => _kafkaConsumerDefinition.IsolationMode switch
        {
            IsolationMode.PerServiceInstance => Guid.NewGuid().ToString(),
            IsolationMode.PerServiceType => _fullServiceDescriptor,
            _ => throw new NotSupportedException($"Isolation level {_kafkaConsumerDefinition.IsolationMode} not currently supported.")
        };

    private IConsumer<TKey, TValue> BuildConsumer(
        string topicToConsume,
        string clientGroupId
    )
    {
        if (_kafkaConsumerDefinition.CreateTopic)
        {
            if (_kafkaConsumerDefinition.UseRegexPatternMatching)
            {
                throw new InvalidOperationException(
                    "Cannot create topic when using regex pattern matching."
                );
            }

            if (!_topicManager.CreateTopic(topicToConsume).Result)
            {
                throw new Exception("Couldn't register topic - oopsie!");
            }
        }

        var consumerBuilder = new ConsumerBuilder<TKey, TValue>(
            new ConsumerConfig
            {
                BootstrapServers = _kafkaServerOptions.BootstrapServers,
                GroupId = clientGroupId,
                MetadataMaxAgeMs = 5000,
                EnableAutoOffsetStore = false,
                EnableAutoCommit = true,
                AutoCommitIntervalMs = 100,
                ApiVersionRequest = false
            });

        if (typeof(TKey) != typeof(Ignore))
        {
            consumerBuilder.SetKeyDeserializer(KafkaDeserializerFactory.GetDeserializer<TKey>());
        }
        if (typeof(TValue) != typeof(Ignore))
        {
            consumerBuilder.SetValueDeserializer(KafkaDeserializerFactory.GetDeserializer<TValue>());
        }
        
        return consumerBuilder.Build();
    }

    private static void MetadataRefresh(Handle handle)
    {
        using var client = new DependentAdminClientBuilder(handle).Build();

        while (true)
        {
            Thread.Sleep(5000);
            Console.WriteLine("Refreshing Metadata...");
            client.GetMetadata(TimeSpan.FromMilliseconds(5000));
        }
    }
}
