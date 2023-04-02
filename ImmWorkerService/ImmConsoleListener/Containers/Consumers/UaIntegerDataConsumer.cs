namespace ImmServiceContainers;

public class UaIntegerDataConsumer : IConsumer<UaIntegerData>
{
    public async Task Consume(ConsumeContext<UaIntegerData> context)
    {
        var message = context.Message;

        Console.WriteLine("Name: {0}, Value: {1}", message.Name, message.Value);
    }
}
