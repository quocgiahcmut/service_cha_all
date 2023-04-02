namespace AssemblySystemServiceContainers;

public class ConfigurationMessageConsumer : IConsumer<ConfigurationMessage>
{
    private readonly MqttClientHelper _mqttClient;

    public ConfigurationMessageConsumer(MqttClientHelper mqttClient)
    {
        _mqttClient = mqttClient;
    }

    public async Task Consume(ConsumeContext<ConfigurationMessage> context)
    {
        Console.WriteLine("ConfigurationMessageReceived");
        var message = context.Message;

        string topic = "AM/" + message.MachineId + "/DAMess";

        var payload = JsonConvert.SerializeObject(message.DesktopAppMessage);

        await _mqttClient.Publish(topic, payload, true);
    }
}
