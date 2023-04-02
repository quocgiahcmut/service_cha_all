using SparkplugNet;
using SparkplugNet.Core;
using SparkplugNet.Core.Node;
using SparkplugNet.Custom;
using VersionB = SparkplugNet.VersionB;
using SparkplugNet.VersionB.Data;

using Serilog;

using Microsoft.Extensions.Options;

namespace ImmWorkerService.ProtocolServices.SparkplugCore;

public class SparkplugClientHelper
{
    private VersionB.SparkplugNode _sparkplugNode;
    private SparkplugNodeOptions _sparkplugNodeOptions;

    public bool IsConnected => _sparkplugNode.IsConnected;

    public SparkplugClientHelper(IOptions<SparkplugClientHelperOptions> options)
    {
        SparkplugClientHelperOptions parameter = options.Value;

        _sparkplugNode = new VersionB.SparkplugNode(new List<Metric>(), Log.Logger);
        _sparkplugNodeOptions = new SparkplugNodeOptions(
            parameter.BrokerAddress,
            parameter.Port,
            parameter.ClientId,
            parameter.UserName,
            parameter.Password,
            parameter.UseTls,
            parameter.ScadaHostIdentifier,
            parameter.GroupIdentifier,
            parameter.EdgeNodeIdentifier,
            TimeSpan.FromSeconds(parameter.ReconnectInterval),
            null,
            null,
            new CancellationToken()
            );
    }

    public async Task StartNodeAsync()
    {
        await _sparkplugNode.Start(_sparkplugNodeOptions);
    }

    public async Task StopNodeAsync()
    {
        await _sparkplugNode.Stop();
    }

    public async Task PublishDeviceBirthMessage(List<Metric> deviceKnownMetrics, string deviceId)
    {
        await _sparkplugNode.PublishDeviceBirthMessage(deviceKnownMetrics, deviceId);
    }

    public async Task PublishDeviceDeathMessage(string deviceId)
    {
        await _sparkplugNode.PublishDeviceDeathMessage(deviceId);
    }

    public async Task PublishDeviceData(List<Metric> deviceMetrics, string deviceId)
    {
        await _sparkplugNode.PublishDeviceData(deviceMetrics, deviceId);
    }
}
