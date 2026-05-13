using System.Text.Json;

namespace SmingCode.Utilities.Kafka.Consumers;
using Config;
using ServiceMetadata;

internal class KafkaConsumer<TKey, TValue>(
    IServiceScopeFactory _serviceScopeFactory,
    ITopicManager _topicManager,
    KafkaConsumerDefinition<TKey, TValue> _kafkaConsumerDefinition,
    IServiceMetadataProvider serviceMetadataProvider,
    KafkaServerOptions _kafkaServerOptions,
    ConsumerMiddlewareHandler middlewareHandler,
    ILogger<KafkaConsumer<TKey, TValue>> _logger
) : IKafkaConsumer
{
    private readonly string _serviceName = serviceMetadataProvider.GetMetadata().ServiceName;
    private readonly JsonSerializerOptions _jsonSerializerOptions = JsonSerializerOptions.Web;

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

        MetadataRefresh(consumer.Handle);

        consumer.Subscribe(topicToConsume);

        var consumerTask = Task.Run(() =>
        {
            try
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation(
                        "Starting consumer on topic {topicToConsume} - {TraceType}",
                        topicToConsume,
                        Constants.CONSUMER_UTILITY_TRACE_TYPE
                    );
                }

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var cr = consumer.Consume(TimeSpan.FromMilliseconds(1000));

                        if (cr is not null && cr.Topic != "__consumer_offsets")
                        {
                            var trackingId = Guid.NewGuid();
                            if (_logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation(
                                    "Message received from topic {topicToConsume}, Beginning processing - {TraceType}",
                                    topicToConsume,
                                    Constants.CONSUMER_UTILITY_TRACE_TYPE
                                );
                                if (_logger.IsEnabled(LogLevel.Trace))
                                {
                                    _logger.LogTrace(
                                        "Message details are: Headers: {Headers}, Key: {Key}, Value: {Value} - {TraceType}",
                                        cr.Message.Headers,
                                        cr.Message.Key,
                                        cr.Message.Value,
                                        Constants.CONSUMER_UTILITY_TRACE_TYPE
                                    );
                                }
                            }

                            Task.Run(async () =>
                            {
                                try
                                {
                                    var result = await ProcessKafkaEvent(
                                        topicToConsume,
                                        cr
                                    );

                                    if (result == KafkaEventResult.Complete)
                                    {
                                        if (_logger.IsEnabled(LogLevel.Information))
                                        {
                                            _logger.LogInformation(
                                                "Kafka consumer for topic {KafkaTopic} successfully consumed message - {TraceType}",
                                                topicToConsume,
                                                Constants.CONSUMER_UTILITY_TRACE_TYPE
                                            );
                                        }

                                        consumer.StoreOffset(cr);
                                    }
                                    else
                                    {
                                        _logger.LogWarning(
                                            "Kafka consumer for topic {KafkaTopic} failed to complete processing - {TraceType}",
                                            topicToConsume,
                                            Constants.CONSUMER_UTILITY_TRACE_TYPE
                                        );
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(
                                        ex,
                                        "Kafka consumer for topic {KafkaTopic} Exception occurred whilst processing message - {TraceType}",
                                        topicToConsume,
                                        Constants.CONSUMER_UTILITY_TRACE_TYPE
                                    );
                                }
                            });
                        }
                    }
                    catch (ConsumeException e)
                    {
                        //We can get this when we consume from a queue not yet created.
                        //The first message sent to that queue will then create the message
                        if (!e.Message.Contains("Broker: Unknown topic or partition"))
                        {
                            _logger.LogWarning(
                                "Subscription to topic '{topicToConsume}' has raised an exception, but will continue until stopped - {TraceType}",
                                topicToConsume,
                                Constants.CONSUMER_UTILITY_TRACE_TYPE
                            );
                        }
                    }
                    catch { }
                }
            }
            catch (OperationCanceledException)
            {
                // Close and Release all the resources held by this consumer
                _logger.LogError(
                    "Subscription to topic '{topicToConsume}' has been stopped.",
                    topicToConsume
                );
                consumer.Close();
                consumer.Dispose();
            }
        }, cancellationToken);
    }

    private async Task<KafkaEventResult> ProcessKafkaEvent(
        string topicConsumed,
        ConsumeResult<string, string> consumeResult
    )
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var key = typeof(TKey) == typeof(Ignore)
            ? default
            : JsonSerializer.Deserialize<TKey>(consumeResult.Message.Key, _jsonSerializerOptions);
        var value = typeof(TValue) == typeof(Ignore)
            ? default
            : JsonSerializer.Deserialize<TValue>(consumeResult.Message.Value, _jsonSerializerOptions);
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(
                "Strongly typed kafka message details are: Key ({KeyType}): {Key}, Value ({ValueType}): {Value} - {TraceType}",
                typeof(TKey),
                key,
                typeof(TValue),
                value,
                Constants.CONSUMER_UTILITY_TRACE_TYPE
            );
        }

        async Task<KafkaEventResult> handlerDelegate(KafkaConsumerContext kafkaConsumerContext) =>
            await _kafkaConsumerDefinition.Handler.Invoke(
                kafkaConsumerContext.ServiceProvider,
                key,
                value
            );

        var context = new KafkaConsumerContext(
            topicConsumed,
            consumeResult.Message.Headers,
            consumeResult.Message.Key,
            typeof(TKey),
            consumeResult.Message.Value,
            typeof(TValue),
            handlerDelegate,
            scope.ServiceProvider
        );

        return await middlewareHandler.RunPipeline(context);
    }

    private string GetTopicToConsume()
        => _kafkaConsumerDefinition.UseRegexPatternMatching
            ? $"^{_kafkaConsumerDefinition.TopicToMatch}"
            : _kafkaConsumerDefinition.TopicToMatch;

    private string GetClientGroupId()
        => _kafkaConsumerDefinition.IsolationMode switch
        {
            IsolationMode.PerServiceInstance => Guid.NewGuid().ToString(),
            IsolationMode.PerServiceType => _serviceName,
            _ => throw new NotSupportedException($"Isolation level {_kafkaConsumerDefinition.IsolationMode} not currently supported.")
        };

    private IConsumer<string, string> BuildConsumer(
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

        var consumerBuilder = new ConsumerBuilder<string, string>(
            new ConsumerConfig
            {
                BootstrapServers = _kafkaServerOptions.BootstrapServers,
                SecurityProtocol = Enum.Parse<SecurityProtocol>(_kafkaServerOptions.SecurityProtocol),
                GroupId = clientGroupId,
                MetadataMaxAgeMs = 5000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoOffsetStore = false,
                EnableAutoCommit = true,
                AutoCommitIntervalMs = 100,
                ApiVersionRequest = false
            });

        return consumerBuilder.Build();
    }

    private static void MetadataRefresh(Handle handle)
    {
        using var client = new DependentAdminClientBuilder(handle).Build();

        client.GetMetadata(TimeSpan.FromMilliseconds(5000));
    }
}
