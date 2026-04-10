using Microsoft.Extensions.DependencyInjection;

namespace SmingCode.Utilities.ProcessTracking;

public interface IProcessTrackingBuilder
{ }

internal interface IProcessTrackingBuilderInternal : IProcessTrackingBuilder
{
    IServiceCollection Services { get; }
}

internal class ProcessTrackingBuilder(
    IServiceCollection services
) : IProcessTrackingBuilderInternal
{
    public IServiceCollection Services => services;
}
