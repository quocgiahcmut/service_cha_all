namespace ImmServiceContainers;

public class SychronizeTimeMessageConsumer : IConsumer<SychronizeTimeMessage>
{
    private readonly MqttClientHelper _mqttClient;

    public SychronizeTimeMessageConsumer(MqttClientHelper mqttClient)
    {
        _mqttClient = mqttClient;
    }

    public async Task Consume(ConsumeContext<SychronizeTimeMessage> context)
    {
        Console.WriteLine("SychronizeTimeMessageReceived");
        var message = context.Message;

        string topic = "IMM/" + message.MachineId + "/SyncTime";

        var payload = JsonConvert.SerializeObject(new SychronizeTimeMqttMessage
        {
            Year = message.Year,
            Month = message.Month,
            Day = message.Day,
            Hour = message.Hour,
            Min = message.Min,
            Sec = message.Sec
        });

        await _mqttClient.Publish(topic, payload, false);
    }
}
