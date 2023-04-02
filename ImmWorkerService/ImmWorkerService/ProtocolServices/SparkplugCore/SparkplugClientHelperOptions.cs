namespace ImmWorkerService.ProtocolServices.SparkplugCore;

public class SparkplugClientHelperOptions
{
    #pragma warning disable CS8618
    public string BrokerAddress { get; set; }
    public int Port { get; set; }
    public string ClientId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool UseTls { get; set; }
    public string ScadaHostIdentifier { get; set; }
    public string GroupIdentifier { get; set; }
    public string EdgeNodeIdentifier { get; set; }
    public double ReconnectInterval { get; set; }
    #pragma warning restore CS8618
}
