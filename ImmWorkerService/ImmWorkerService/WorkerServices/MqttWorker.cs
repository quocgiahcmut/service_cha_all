namespace ImmWorkerService.WorkerServices;

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
        _previousMachineStatus.Add("L6", EMachineStatus.PowerOff);
        _previousMachineStatus.Add("L7", EMachineStatus.PowerOff);
        _previousMachineStatus.Add("L8", EMachineStatus.PowerOff);
        _previousMachineStatus.Add("L9", EMachineStatus.PowerOff);
        _previousMachineStatus.Add("L10", EMachineStatus.PowerOff);

        _mqttClient.ApplicationMessageReceivedEvent = MqttClient_ApplicationMessageReceivedAsync;
        await _mqttClient.ConnectAsync();
        _logger.LogInformation("Connected Broker: {1}", _mqttClient.IsConnected);

        //L6
        await _mqttClient.Subscribe("IMM/L6/CycleMessage");
        await _mqttClient.Subscribe("IMM/L6/MachineStatus");
        await _mqttClient.Subscribe("IMM/L6/Feedback");
        //L7
        await _mqttClient.Subscribe("IMM/L7/CycleMessage");
        await _mqttClient.Subscribe("IMM/L7/MachineStatus");
        await _mqttClient.Subscribe("IMM/L7/Feedback");
        //L8
        await _mqttClient.Subscribe("IMM/L8/CycleMessage");
        await _mqttClient.Subscribe("IMM/L8/MachineStatus");
        await _mqttClient.Subscribe("IMM/L8/Feedback");
        //L9
        await _mqttClient.Subscribe("IMM/L9/CycleMessage");
        await _mqttClient.Subscribe("IMM/L9/MachineStatus");
        await _mqttClient.Subscribe("IMM/L9/Feedback");
        //L10
        await _mqttClient.Subscribe("IMM/L10/CycleMessage");
        await _mqttClient.Subscribe("IMM/L10/MachineStatus");
        await _mqttClient.Subscribe("IMM/L10/Feedback");
    }

    public async Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        if (e.ApplicationMessage.Dup)
        {
            return;
        }

        var topic = e.ApplicationMessage.Topic;
        var payloadMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

        _logger.LogInformation("topic: " + topic);
        _logger.LogInformation("message:\r\n" + payloadMessage);

        var machineId = topic.Split('/')[1];
        var messageType = topic.Split('/')[2];

        switch (machineId)
        {
            case "L6":
                {
                    await HandleMessageReceived(machineId, messageType, payloadMessage);
                }
                break;
            case "L7":
                {
                    await HandleMessageReceived(machineId, messageType, payloadMessage);
                }
                break;
            case "L8":
                {
                    await HandleMessageReceived(machineId, messageType, payloadMessage);
                }
                break;
            case "L9":
                {
                    await HandleMessageReceived(machineId, messageType, payloadMessage);
                }
                break;
            case "L10":
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
            case "CycleMessage":
                {
                    await CycleMessageReceived(machineId, payloadMessage);
                }
                break;
            case "MachineStatus":
                {
                    await MachineStatusReceived(machineId, payloadMessage);
                }
                break;
            case "Feedback":
                {
                    await FeedbackReceived(machineId, payloadMessage);
                }
                break;
        }
    }

    private async Task CycleMessageReceived(string machineId, string payloadMessage)
    {
        try
        {
            CycleMqttMessage? cycleMqttMessage = JsonConvert.DeserializeObject<CycleMqttMessage>(payloadMessage);

            var listMetrics = new List<Metric>
            {
                new Metric { Name = machineId + ".CycleTime", ValueCase = DataType.Double, DoubleValue = (double)cycleMqttMessage.CycleTime },
                new Metric { Name = machineId + ".OpenTime", ValueCase = DataType.Double, DoubleValue = (double)cycleMqttMessage.OpenTime },
                new Metric { Name = machineId + ".CounterShot", ValueCase = DataType.Int32, IntValue = (uint)cycleMqttMessage.CounterShot },
                new Metric { Name = machineId + ".SetCycle", ValueCase = DataType.Double, DoubleValue = (double)cycleMqttMessage.SetCycle }
            };
            await _sparkplugClient.PublishDeviceData(listMetrics, machineId.ToLower());

            var cycleMessage = new CycleMessage
            {
                MachineId = machineId,
                Timestamp = cycleMqttMessage.Timestamp,
                CycleTime = cycleMqttMessage.CycleTime,
                OpenTime = cycleMqttMessage.OpenTime,
                Mode = cycleMqttMessage.Mode,
                CounterShot = cycleMqttMessage.CounterShot,
                MoldId = cycleMqttMessage.MoldId,
                ProductId = cycleMqttMessage.ProductId,
                SetCycle = cycleMqttMessage.SetCycle
            };
            await _busControl.Publish<CycleMessage>(cycleMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    private async Task MachineStatusReceived(string machineId, string payloadMessage)
    {
        try
        {
            MachineMqttMessage? statusMqtt = JsonConvert.DeserializeObject<MachineMqttMessage>(payloadMessage);

            if (statusMqtt.MachineStatus != _previousMachineStatus[machineId] && statusMqtt.MachineStatus != EMachineStatus.PowerOff)
            {
                List<Metric> knownMetrics = new List<Metric>
                {
                    new Metric { Name = machineId + ".CycleTime", ValueCase = DataType.Double, DoubleValue = 0.0 },
                    new Metric { Name = machineId + ".OpenTime", ValueCase = DataType.Double, DoubleValue = 0.0 },
                    new Metric { Name = machineId + ".CounterShot", ValueCase = DataType.Int32, IntValue = 0 },
                    new Metric { Name = machineId + ".SetCycle", ValueCase = DataType.Double, DoubleValue = 0.0 },
                    new Metric { Name = machineId + ".MachineStatus", ValueCase = DataType.Int32, IntValue = 0 }
                };

                await _sparkplugClient.PublishDeviceBirthMessage(knownMetrics, machineId.ToLower());
            }
            if (statusMqtt.MachineStatus != _previousMachineStatus[machineId] && statusMqtt.MachineStatus == EMachineStatus.PowerOff)
            {
                await _sparkplugClient.PublishDeviceDeathMessage(machineId.ToLower());
            }
            _previousMachineStatus[machineId] = statusMqtt.MachineStatus;

            var listMetrics = new List<Metric>
            {
                new Metric { Name = machineId +".MachineStatus", ValueCase = DataType.Int32, IntValue = (uint)statusMqtt.MachineStatus }
            };
            await _sparkplugClient.PublishDeviceData(listMetrics, machineId.ToLower());

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
        }
    }

    private async Task FeedbackReceived(string machineId, string payloadMessage)
    {
        try
        {
            FeedbackMqttMessage? feedbackMqtt = JsonConvert.DeserializeObject<FeedbackMqttMessage>(payloadMessage);

            var feedbackMessage = new FeedbackMessage
            {
                MachineId = machineId,
                Mess = feedbackMqtt.Mess
            };
            await _busControl.Publish<FeedbackMessage>(feedbackMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
