namespace ImmWorkerService.ProtocolServices.MqttCore;

public class MqttClientHelperOptions
{
    #pragma warning disable CS8618
    public int CommunicationTimeout { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public int KeepAliveInterval { get; set; }
    public string ClientId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    #pragma warning restore CS8618
}
