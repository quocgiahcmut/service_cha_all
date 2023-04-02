namespace ImmServiceContainers;

public class UaBooleanDataConsumer : IConsumer<UaBooleanData>
{
    public async Task Consume(ConsumeContext<UaBooleanData> context)
    {
        var message = context.Message;

        Console.WriteLine("Name: {0}, Value: {1}", message.Name, message.Value);
    }
}
