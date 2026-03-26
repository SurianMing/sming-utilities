namespace SurianMing.Utilities.Kafka;

public static class KafkaHost
{
    public static KafkaApplicationBuilder CreateApplicationBuilder() => new();
    public static KafkaApplicationBuilder CreateApplicationBuilder(string[]? args) => new (args);
}
