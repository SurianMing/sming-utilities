using Microsoft.Extensions.DependencyInjection;

namespace SmingCode.Utilities.Kafka.MinimalConsumers;

public interface IMinimalConsumer
{
    void Consume(IServiceCollection services);
}
