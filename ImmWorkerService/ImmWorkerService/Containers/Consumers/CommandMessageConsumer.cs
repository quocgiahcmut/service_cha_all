namespace ImmServiceContainers;

public class CommandMessageConsumer : IConsumer<CommandMessage>
{
    private readonly MqttClientHelper _mqttClient;

    public CommandMessageConsumer(MqttClientHelper mqttClient)
    {
        _mqttClient = mqttClient;
    }

    public async Task Consume(ConsumeContext<CommandMessage> context)
    {
        Console.WriteLine("CommandMessageReceived");
        var message = context.Message;

        string topic = "IMM/" + message.MachineId + "/DAMess";

        var payload = JsonConvert.SerializeObject(new CommandMqttMessage
        {
            Timestamp = message.Timestamp,
            Command = message.Command
        });

        await _mqttClient.Publish(topic, payload, false);
    }
}
