using Microsoft.Extensions.Logging;

namespace shared_logic;

public class SimpleService(ILogger<SimpleService> logger) : ISimpleService
{
    public async Task<bool> DoLongRunningTaskAsync()
    {
        // Simulate a long-running task
        logger.LogInformation("Starting long-running task...");
        await Task.Delay(5000);
        logger.LogInformation("Finished long-running task.");
        return true;
    }

    public async Task<bool> DoShortRunningTaskAsync()
    {
        // Simulate a short-running task
        logger.LogInformation("Starting short-running task...");
        await Task.Delay(1000);
        logger.LogInformation("Finished short-running task.");
        return true;
    }
}
