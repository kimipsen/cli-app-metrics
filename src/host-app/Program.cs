using host_app;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string serviceName = "host-app";
const string serviceVersion = "1.0.0";

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddOpenTelemetry()
            .WithTracing(traces => traces
                .AddSource("SampleActivitySource")
                .ConfigureResource(config => config.AddService(serviceName, serviceVersion: serviceVersion))
                .AddConsoleExporter()
                .AddOtlpExporter()
            )
            .WithMetrics(metrics => metrics
                .ConfigureResource(config => config.AddService(serviceName, serviceVersion: serviceVersion))
                .AddMeter("SampleMeter")
                .AddConsoleExporter()
                .AddOtlpExporter()
            );

        // Register Meter and ActivitySource as singletons after OTel pipeline is configured
        services.AddSingleton(new System.Diagnostics.Metrics.Meter("SampleMeter", "1.0.0"));
        services.AddSingleton(new System.Diagnostics.ActivitySource("SampleActivitySource", "1.0.0"));
        services.AddScoped<shared_logic.ISimpleService, shared_logic.SimpleService>();
        services.AddHostedService<Worker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(ResourceBuilder
                .CreateDefault()
                .AddService(serviceName, serviceVersion: serviceVersion))
                .AddOtlpExporter();
        })
        .SetMinimumLevel(LogLevel.Debug);
    });

await host.RunConsoleAsync();