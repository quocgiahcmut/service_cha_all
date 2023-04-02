using SimpleTcp;

using Microsoft.Extensions.Options;

namespace PackingSystemWorkerService.ProtocolServices.TcpCore;

public class TcpServerHelper
{
    private SimpleTcpServer _server;

    public Func<ClientConnectedEventArgs, Task> ClientConnectedEvent;
    public Func<ClientDisconnectedEventArgs, Task> ClientDisconnectedEvent;
    public Func<DataReceivedEventArgs, Task> DataReceivedEvent;

    public string IpAddress { get; set; }
    public Dictionary<string, string> ClientIpPort = new Dictionary<string, string>();
    public bool Started => _server.IsListening;

    public TcpServerHelper(string ipAddress)
    {
        IpAddress = ipAddress;
    }

    public TcpServerHelper(IOptions<TcpServerHelperOptions> options)
    {
        IpAddress = options.Value.IpAddress;
    }

    public bool Start()
    {
        try
        {
            _server = new SimpleTcpServer(IpAddress);

            _server.Start();
            _server.Events.DataReceived += OnClientDataReceived;
            _server.Events.ClientConnected += OnClientConnected;
            _server.Events.ClientDisconnected += OnClientDisconnected;

            return true;
        }
        catch
        { return false; }
    }

    public async Task SendAsync(string clientIpPort, string data)
    {
        await _server.SendAsync(clientIpPort, data);
    }

    public void OnClientConnected(object sender, ClientConnectedEventArgs eventArgs)
    {
        ClientConnectedEvent.Invoke(eventArgs);
    }

    public void OnClientDisconnected(object sender, ClientDisconnectedEventArgs eventArgs)
    {
        ClientDisconnectedEvent.Invoke(eventArgs);
    }

    public void OnClientDataReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        DataReceivedEvent.Invoke(eventArgs);
    }
}