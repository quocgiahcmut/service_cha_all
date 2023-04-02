namespace PackingSystemServiceContainers;

public class EspConfigurationMessageConsumer : IConsumer<EspConfigurationMessage>
{
    private readonly MqttClientHelper _mqttClient;

    public EspConfigurationMessageConsumer(MqttClientHelper mqttClient)
    {
        _mqttClient = mqttClient;
    }

    public async Task Consume(ConsumeContext<EspConfigurationMessage> context)
    {
        Console.WriteLine("EspConfigurationMessageReceived");
        var message = context.Message;

        string topic = "PS/" + message.MachineId + "/DAMess";

        var payload = JsonConvert.SerializeObject(message.DesktopAppMessage);

        await _mqttClient.Publish(topic, payload);
    }
}
