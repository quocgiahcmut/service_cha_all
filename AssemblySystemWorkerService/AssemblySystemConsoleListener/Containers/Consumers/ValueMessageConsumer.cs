namespace AssemblySystemServiceContainers;

public class ValueMessageConsumer : IConsumer<ValueMessage>
{
    public async Task Consume(ConsumeContext<ValueMessage> context)
    {
        var message = context.Message;

        Console.WriteLine("ValueMessage: MachineId: {0}, Timestamp: {1}, ItemId: {2}, CurrentValue: {3}", message.MachineId, message.Timestamp, message.ItemId, message.CurrentValue);
    }
}
