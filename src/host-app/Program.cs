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
        services.AddScoped<shared_logic.ISimpleService, shared_logic.SimpleService>();

        services.AddOpenTelemetry()
            .WithTracing(traces => traces
                .AddHttpClientInstrumentation()
                .AddOtlpExporter()
            )
            .WithMetrics(metrics => metrics
                .AddHttpClientInstrumentation()
                .AddOtlpExporter()
            );
    })
    .ConfigureLogging(logging =>
    {
        logging.SetMinimumLevel(LogLevel.Debug);
        logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion: serviceVersion));
            options.AddOtlpExporter();
        });
    })
    .Build();

var service = host.Services.GetRequiredService<shared_logic.ISimpleService>();

// Execute the long-running and short-running tasks concurrently
// and wait for both to complete.
// Build the host application with the necessary services.
// This demonstrates the use of async methods in a console application.
// The long-running task simulates a delay, while the short-running task completes quickly.
var longTask = service.DoLongRunningTaskAsync();
var shortTask = service.DoShortRunningTaskAsync();

await Task.WhenAll(longTask, shortTask);

// Ensure OpenTelemetry exporters flush before exit (important for short-lived console apps)
await Task.Delay(6000);
