using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmServiceContainers;
public class MachineMessageConsumer : IConsumer<MachineMessage>
{
    public async Task Consume(ConsumeContext<MachineMessage> context)
    {
        var message = context.Message;

        Console.WriteLine("MachineMessage: MachineId: {0}, Timestamp: {1}, MachineStatus: {2}", message.MachineId, message.Timestamp, message.MachineStatus);
    }
}
