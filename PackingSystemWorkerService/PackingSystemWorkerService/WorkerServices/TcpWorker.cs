namespace PackingSystemWorkerService.WorkerServices;

public class TcpWorker : BackgroundService
{
    private readonly ILogger<TcpWorker> _logger;
    private readonly IBusControl _busControl;
    private readonly TcpServerHelper _tcpServer;
    private readonly SparkplugClientHelper _sparkplugClient;

    public TcpWorker(ILogger<TcpWorker> logger, IBusControl busControl, TcpServerHelper tcpServer, SparkplugClientHelper sparkplugClient)
    {
        _logger = logger;
        _busControl = busControl;
        _tcpServer = tcpServer;
        _sparkplugClient = sparkplugClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _tcpServer.ClientConnectedEvent = ClientConnected;
        _tcpServer.ClientDisconnectedEvent = ClientDisconnected;
        _tcpServer.DataReceivedEvent = DataReceived;

        _tcpServer.Start();
        _logger.LogInformation("Tcp Server Started: {1}", _tcpServer.Started);
    }

    public async Task ClientConnected(ClientConnectedEventArgs e)
    {
        //Do Some Thing
    }

    public async Task ClientDisconnected(ClientDisconnectedEventArgs e)
    {
        _logger.LogInformation($"[{e.IpPort}] client disconnected: {e.Reason}");

        var machineId = _tcpServer.ClientIpPort.FirstOrDefault(ipp => ipp.Value == e.IpPort).Key;

        _tcpServer.ClientIpPort.Remove(machineId);
        //await _sparkplugClient.PublishDeviceDeathMessage(machineId);

        await _busControl.Publish<MachineMessage>(new MachineMessage
        {
            MachineId = machineId,
            MachineStatus = EMachineStatus.Disconnected,
            Timestamp = DateTime.Now
        });
    }

    public async Task DataReceived(DataReceivedEventArgs e)
    {
        _logger.LogInformation($"[{e.IpPort}]: {Encoding.UTF8.GetString(e.Data)}");

        var data = Encoding.UTF8.GetString(e.Data).Split('|');

        switch (data[0])
        {
            case "Message": //Message|DG1|timestamp|itemid|cp|ep
                {
                    await MessageReceivedHandler(e, data[1], data[2], data[3], data[4], data[5]);
                }
                break;
            case "Connect": //Connect|DG1
                {
                    await ConnectedHandler(e, data[1]);
                }
                break;
            case "Idle": //Idle|DG1|bool
                {
                    await IdleHandler(e, data[1], data[2]);
                }
                break;
            case "Error": //Error|DG1|mayngu
                {

                }
                break;
        }
    }

    public async Task MessageReceivedHandler(DataReceivedEventArgs e, string machineId, string timestamp, string itemId, string completedProduct, string errorProduct)
    {
        try
        {
            var listMetrics = new List<Metric>
            {
                new Metric { Name = machineId + ".CompletedProduct", ValueCase = DataType.Int32, IntValue = uint.Parse(completedProduct) },
                new Metric { Name = machineId + ".ErrorProduct", ValueCase = DataType.Int32, IntValue = uint.Parse(errorProduct) }
            };
            //await _sparkplugClient.PublishDeviceData(listMetrics, machineId);

            var valueMessage = new ValueMessage
            {
                MachineId = machineId,
                Timestamp = DateTime.Parse(timestamp),
                ItemId = int.Parse(itemId),
                CompletedProduct = int.Parse(completedProduct),
                ErrorProduct = int.Parse(errorProduct)
            };
            await _busControl.Publish<ValueMessage>(valueMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public async Task ConnectedHandler(DataReceivedEventArgs e, string machineId)
    {
        _logger.LogInformation($"[{e.IpPort}] client connected with machineId: {machineId}");
        _tcpServer.ClientIpPort.Add(machineId, e.IpPort);

        List<Metric> knownMetrics = new List<Metric>
        {
            new Metric { Name = machineId + ".CompletedProduct", ValueCase = DataType.Int32, IntValue = 0 },
            new Metric { Name = machineId + ".ErrorProduct", ValueCase = DataType.Int32, IntValue = 0 },
            new Metric { Name = machineId + ".MachineStatus", ValueCase = DataType.Int32, IntValue = 0 }
        };

        //await _sparkplugClient.PublishDeviceBirthMessage(knownMetrics, machineId);

        await _busControl.Publish<MachineMessage>(new MachineMessage
        {
            MachineId = machineId,
            MachineStatus = EMachineStatus.Connected,
            Timestamp = DateTime.Now
        });
    }

    public async Task IdleHandler(DataReceivedEventArgs e, string machineId, string isIdle)
    {
        bool idle = bool.Parse(isIdle);
        if (idle)
        {
            await _busControl.Publish<MachineMessage>(new MachineMessage
            {
                MachineId = machineId,
                MachineStatus = EMachineStatus.IdleOn,
                Timestamp = DateTime.Now
            });
        }
        else
        {
            await _busControl.Publish<MachineMessage>(new MachineMessage
            {
                MachineId = machineId,
                MachineStatus = EMachineStatus.IdleOff,
                Timestamp = DateTime.Now
            });
        }
    }
}
