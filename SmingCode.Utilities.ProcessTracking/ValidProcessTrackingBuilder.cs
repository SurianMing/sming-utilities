using Microsoft.Extensions.DependencyInjection;

namespace SmingCode.Utilities.ProcessTracking;

public interface IValidProcessTrackingBuilder : IProcessTrackingBuilder { }

internal class ValidProcessTrackingBuilder(
    IServiceCollection services
) : ProcessTrackingBuilder(
    services
), IValidProcessTrackingBuilder
{ }
