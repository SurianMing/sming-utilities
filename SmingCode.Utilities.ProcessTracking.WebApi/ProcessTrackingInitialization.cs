using Microsoft.AspNetCore.Builder;

namespace SmingCode.Utilities.ProcessTracking.WebApi;
using StartupProcesses;

internal class ProcessTrackingInitialization : IServiceInitializer
{
    public Delegate ServiceInitializer => (IApplicationBuilder applicationBuilder) =>
    {
        applicationBuilder.UseMiddleware<ProcessTrackingHeaderMiddleware>();        
    };
}