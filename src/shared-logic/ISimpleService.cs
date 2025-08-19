namespace shared_logic;

public interface ISimpleService
{
    Task<bool> DoLongRunningTaskAsync();
    Task<bool> DoShortRunningTaskAsync();
    Task IncrementSampleMeter();
    Task TraceSampleOperationAsync();
}
