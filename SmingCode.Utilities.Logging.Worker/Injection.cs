using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace SmingCode.Utilities.Logging.Worker;

public static class Injection
{
    public static IHostApplicationBuilder InitializeLogging(
        this IHostApplicationBuilder builder
    )
    {
        builder.Services.ConfigureOpenTelemetryTracerProvider(provider =>
            provider.AddAzureMonitorTraceExporter()
        );

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.AddAzureMonitorLogExporter();
            logging.IncludeScopes = true;
        });

        return builder;
    }
}
