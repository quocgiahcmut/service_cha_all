namespace PackingSystemWorkerService.WorkerServices;

public class SparkplugWorker : BackgroundService
{
    private readonly ILogger<SparkplugWorker> _logger;
    private readonly SparkplugClientHelper _sparkplugClient;

    public SparkplugWorker(ILogger<SparkplugWorker> logger, SparkplugClientHelper sparkplugClient)
    {
        _logger = logger;
        _sparkplugClient = sparkplugClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _sparkplugClient.StartNodeAsync();
        _logger.LogInformation("Sparkplug Connected Broker: {1}", _sparkplugClient.IsConnected);
    }
}
