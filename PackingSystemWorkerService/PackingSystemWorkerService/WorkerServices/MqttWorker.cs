namespace PackingSystemWorkerService.WorkerServices;

public class MqttWorker : BackgroundService
{
    private readonly ILogger<MqttWorker> _logger;
    private readonly IBusControl _busControl;
    private readonly MqttClientHelper _mqttClient;
    private readonly SparkplugClientHelper _sparkplugClient;
    public Dictionary<string, EMachineStatus> _previousMachineStatus = new Dictionary<string, EMachineStatus>();

    public MqttWorker(ILogger<MqttWorker> logger, IBusControl busControl, MqttClientHelper mqttClient, SparkplugClientHelper sparkplugClient)
    {
        _logger = logger;
        _busControl = busControl;
        _mqttClient = mqttClient;
        _sparkplugClient = sparkplugClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //_previousMachineStatus.Add("DG2", EMachineStatus.PowerOff);

        /*await _mqttClient.ConnectAsync();
        _logger.LogInformation("Connected Broker: {1}", _mqttClient.IsConnected);

        await _mqttClient.Subscribe("PS/DG2/ValueMessage");
        await _mqttClient.Subscribe("PS/DG2/MachineStatus");
        _mqttClient.ApplicationMessageReceivedEvent = MqttClient_ApplicationMessageReceivedAsync;*/
    }

    public async Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        var topic = e.ApplicationMessage.Topic;
        var payloadMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

        _logger.LogInformation("topic: " + topic);
        _logger.LogInformation("message:\r\n" + payloadMessage);

        var machineId = topic.Split('/')[1];
        var messageType = topic.Split('/')[2];

        switch (machineId)
        {
            case "DG2":
                {
                    await HandleMessageReceived(machineId, messageType, payloadMessage);
                }
                break;
        }
    }

    private async Task HandleMessageReceived(string machineId, string messageType, string payloadMessage)
    {
        switch (messageType)
        {
            case "ValueMessage":
                {
                    await ValueMessageReceived(machineId, payloadMessage);
                }
                break;
            case "MachineStatus":
                {
                    await MachineStatusReceived(machineId, payloadMessage);
                }
                break;
        }
    }

    private async Task ValueMessageReceived(string machineId, string payloadMessage)
    {
        try
        {
            ValueMqttMessage? valueMqtt = JsonConvert.DeserializeObject<ValueMqttMessage>(payloadMessage);

            var listMetrics = new List<Metric>
            {
                new Metric { Name = machineId + ".CompletedProduct", ValueCase = DataType.Int32, IntValue = (uint)valueMqtt.CompletedProduct },
                new Metric { Name = machineId + ".ErrorProduct", ValueCase = DataType.Int32, IntValue = (uint)valueMqtt.ErrorProduct }
            };
            await _sparkplugClient.PublishDeviceData(listMetrics, machineId);

            var valueMessage = new ValueMessage
            {
                MachineId = machineId,
                Timestamp = valueMqtt.Timestamp,
                ItemId = valueMqtt.ItemId,
                CompletedProduct = valueMqtt.CompletedProduct,
                ErrorProduct = valueMqtt.ErrorProduct
            };
            await _busControl.Publish<ValueMessage>(valueMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    private async Task MachineStatusReceived(string machineId, string payloadMessage)
    {
        /*try
        {
            MachineMqttMessage? statusMqtt = JsonConvert.DeserializeObject<MachineMqttMessage>(payloadMessage);

            if (statusMqtt.MachineStatus != _previousMachineStatus[machineId] && statusMqtt.MachineStatus == EMachineStatus.PowerOn)
            {
                List<Metric> knownMetrics = new List<Metric>
                {
                    new Metric { Name = machineId + ".CompletedProduct", ValueCase = DataType.Int32, IntValue = 0 },
                    new Metric { Name = machineId + ".ErrorProduct", ValueCase = DataType.Int32, IntValue = 0 },
                    new Metric { Name = machineId + ".MachineStatus", ValueCase = DataType.Int32, IntValue = 0 }
                };

                await _sparkplugClient.PublishDeviceBirthMessage(knownMetrics, machineId);
            }
            if (statusMqtt.MachineStatus != _previousMachineStatus[machineId] && statusMqtt.MachineStatus == EMachineStatus.PowerOff)
            {
                await _sparkplugClient.PublishDeviceDeathMessage(machineId);
            }
            _previousMachineStatus[machineId] = statusMqtt.MachineStatus;

            var listMetrics = new List<Metric>
            {
                new Metric { Name = machineId +".MachineStatus", ValueCase = DataType.Int32, IntValue = (uint)statusMqtt.MachineStatus }
            };
            await _sparkplugClient.PublishDeviceData(listMetrics, machineId);

            var machineMessage = new MachineMessage
            {
                MachineId = machineId,
                Timestamp = statusMqtt.Timestamp,
                MachineStatus = statusMqtt.MachineStatus
            };
            await _busControl.Publish<MachineMessage>(machineMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }*/
    }
}
