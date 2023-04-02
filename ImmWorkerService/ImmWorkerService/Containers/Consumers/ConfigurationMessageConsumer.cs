namespace ImmServiceContainers;

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

        string topic = "IMM/" + message.MachineId + "/ConfigMess";

        var payload = JsonConvert.SerializeObject(new ConfigurationMqttMessage
        {
            Timestamp = message.Timestamp,
            MoldId = message.MoldId,
            ProductId = message.ProductId,
            CycleTime = message.CycleTime
        });

        await _mqttClient.Publish(topic, payload, true);
    }
}