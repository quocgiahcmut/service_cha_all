namespace ImmServiceContainers;

public class CycleMessageConsumer : IConsumer<CycleMessage>
{
    public async Task Consume(ConsumeContext<CycleMessage> context)
    {
        var message = context.Message;

        Console.WriteLine("Timestamp: {0}\r\nCycleTime: {1}\r\nOpenTime: {2}\r\nMode: {3}\r\nCounterShot: {4}\r\nMoldId: {5}\r\nSetCycle: {6}\r\n", message.Timestamp, message.CycleTime, message.OpenTime, message.Mode, message.CounterShot, message.MoldId, message.SetCycle);
    }
}
