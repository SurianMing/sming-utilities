namespace SmingCode.Utilities.Kafka;

internal class KafkaServerOptions
{
    public required string BootstrapServers { get; set; }
    public required string SecurityProtocol { get; set; }
    public int LivenessLogIntervalSeconds { get; set; } = 30;
    public string? SaslMechanism { get; set; }
    public string? SaslUsername { get; set; }
    public string? SaslPassword { get; set; }
}