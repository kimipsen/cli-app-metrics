using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;
using System.Diagnostics;

namespace shared_logic;

public class SimpleService(ILogger<SimpleService> logger, Meter meter, ActivitySource activitySource) : ISimpleService
{
    private readonly ILogger<SimpleService> logger = logger;
    private readonly Counter<long> sampleCounter = meter.CreateCounter<long>("sample_counter");
    private readonly ActivitySource activitySource = activitySource;

    public async Task<bool> DoLongRunningTaskAsync()
    {
        logger.LogInformation("Starting long-running task...");
        await Task.Delay(5000);
        logger.LogInformation("Finished long-running task.");
        return true;
    }

    public async Task<bool> DoShortRunningTaskAsync()
    {
        logger.LogInformation("Starting short-running task...");
        await Task.Delay(1000);
        logger.LogInformation("Finished short-running task.");
        return true;
    }

    public async Task IncrementSampleMeter()
    {
        for (int i = 0; i < 10; i++)
        {
            sampleCounter.Add(Random.Shared.Next(1, 10));
            logger.LogInformation("Sample meter incremented.");
            await Task.Delay(100);
        }
    }

    public async Task TraceSampleOperationAsync()
    {
        int n = 6; // Example input
        logger.LogInformation("Calculating Fibonacci({N}) with tracing...", n);
        int result = await Task.Run(() => TracedFibonacci(n));
        logger.LogInformation("Fibonacci({N}) = {Result}", n, result);
    }

    private int TracedFibonacci(int n)
    {
        using var activity = activitySource.StartActivity($"Fibonacci({n})");
        if (n <= 1)
        {
            activity?.SetTag("result", n);
            return n;
        }
        int value = TracedFibonacci(n - 1) + TracedFibonacci(n - 2);
        activity?.SetTag("result", value);
        return value;
    }
}
