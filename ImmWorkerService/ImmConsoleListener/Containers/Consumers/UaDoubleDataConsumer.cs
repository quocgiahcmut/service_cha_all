namespace ImmServiceContainers;

public class UaDoubleDataConsumer : IConsumer<UaDoubleData>
{
    public async Task Consume(ConsumeContext<UaDoubleData> context)
    {
        var message = context.Message;

        Console.WriteLine("Name: {0}, Value: {1}", message.Name, message.Value);
    }
}
