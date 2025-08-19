using Microsoft.Extensions.Hosting;
using shared_logic;

namespace host_app;

public class Worker(ISimpleService simpleService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await simpleService.DoShortRunningTaskAsync();
        await simpleService.DoLongRunningTaskAsync();
        await simpleService.IncrementSampleMeter();
        await simpleService.TraceSampleOperationAsync();
        await Task.Delay(2000, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
