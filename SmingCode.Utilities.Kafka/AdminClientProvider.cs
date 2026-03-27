namespace SmingCode.Utilities.Kafka;

internal class AdminClientProvider(
    KafkaServerOptions _kafkaServerOptions
) : IAdminClientProvider
{
    public IAdminClient GetAdminClient()
    {
        var adminClientConfig = new AdminClientConfig
        {
            BootstrapServers = _kafkaServerOptions.BootstrapServers,
            SecurityProtocol = Enum.Parse<SecurityProtocol>(_kafkaServerOptions.SecurityProtocol)
        };

        if (!string.IsNullOrEmpty(_kafkaServerOptions.SaslMechanism))
        {
            adminClientConfig.SaslMechanism = Enum.Parse<SaslMechanism>(_kafkaServerOptions.SaslMechanism);
            adminClientConfig.SaslUsername = _kafkaServerOptions.SaslUsername;
            adminClientConfig.SaslPassword = _kafkaServerOptions.SaslPassword;
        }

        return new AdminClientBuilder(
            adminClientConfig
        ).Build();
    }
}