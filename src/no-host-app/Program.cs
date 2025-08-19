using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string serviceName = "no-host-app";
const string serviceVersion = "1.0.0";

var meter = new System.Diagnostics.Metrics.Meter("SampleMeter", "1.0.0");
var activitySource = new System.Diagnostics.ActivitySource("SampleActivitySource", "1.0.0");

// Configure OpenTelemetry MeterProvider and TracerProvider
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(ResourceBuilder
        .CreateDefault()
        .AddService(serviceName, serviceVersion: serviceVersion))
    .AddMeter("SampleMeter")
    .AddConsoleExporter()
    .AddOtlpExporter()
    .Build();

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder
        .CreateDefault()
        .AddService(serviceName, serviceVersion: serviceVersion))
    .AddSource("SampleActivitySource")
    .AddConsoleExporter()
    .AddOtlpExporter()
    .Build();

// Create LoggerFactory and configure OpenTelemetry
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(LogLevel.Debug)
    .AddConsole() // Add console output for local visibility
    .AddOpenTelemetry(options =>
        {
            options
                .SetResourceBuilder(ResourceBuilder
                    .CreateDefault()
                    .AddService(serviceName, serviceVersion: serviceVersion))
                .AddOtlpExporter();
        });
});



// Create logger for SimpleService
var logger = loggerFactory.CreateLogger<shared_logic.SimpleService>();
var service = new shared_logic.SimpleService(logger, meter, activitySource);

var longTask = service.DoLongRunningTaskAsync();
var shortTask = service.DoShortRunningTaskAsync();
var meterTask = service.IncrementSampleMeter();
var traceTask = service.TraceSampleOperationAsync();

await Task.WhenAll(longTask, shortTask, meterTask, traceTask);

await Task.Delay(6000);

