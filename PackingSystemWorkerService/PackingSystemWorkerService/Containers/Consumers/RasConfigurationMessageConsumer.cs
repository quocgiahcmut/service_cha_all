namespace PackingSystemServiceContainers;

public class RasConfigurationMessageConsumer : IConsumer<RasConfigurationMessage>
{
    private readonly TcpServerHelper _tcpServer;

    public RasConfigurationMessageConsumer(TcpServerHelper tcpServer)
    {
        _tcpServer = tcpServer;
    }

    public async Task Consume(ConsumeContext<RasConfigurationMessage> context)
    {
        Console.WriteLine("RasConfigurationMessageReceived");
        var message = context.Message;

        var tcpMessage = TcpMessageSerialize(message);

        await _tcpServer.SendAsync(_tcpServer.ClientIpPort[message.MachineId], tcpMessage);
    }

    public string TcpMessageSerialize(RasConfigurationMessage config)
    {
        string items = "";
        foreach (var item in config.Items)
        {
            string boms = "";
            foreach (var bom in item.Boms) 
            { boms += bom + "-"; }

            items += item.SetpointTotal.ToString() + ","
                   + item.ProductId + ","
                   + item.ProductName + ","
                   + boms.Remove(boms.Length - 1, 1) + ","
                   + item.ProductMass.ToString() + ","
                   + item.Standard + ","
                   + item.CompletedProduct.ToString() + ","
                   + item.ErrorProduct.ToString() + "|";
        }

        string mess = "Config" + "|"
                    + config.Timestamp.ToString() + "|"
                    + config.Quantity.ToString() + "|"
                    + items.Remove(items.Length - 1, 1);

        return mess;
    }
}
