IBusControl busControl = Bus.Factory.CreateUsingGrpc(x =>
{
    x.Host(h =>
    {
        h.Host = "127.0.0.1";
        h.Port = 8182;

        h.AddServer(new Uri("http://127.0.0.1:8181"));
    });

    x.ReceiveEndpoint("event-listener", e =>
    {
        e.Consumer<ValueMessageConsumer>();
        e.Consumer<MachineMessageConsumer>();
    });
});

await busControl.StartAsync();

try
{
    Console.WriteLine("Dang nge ne");

    while(true)
    {
        var endpoint = await busControl.GetSendEndpoint(new Uri("http://127.0.0.1:8181/send-config"));

        List<ItemConfigurationMessage> configs = new List<ItemConfigurationMessage>
        {
            new ItemConfigurationMessage { Timestamp = DateTime.UtcNow, Quantity = 2, ItemId = 1, SetpointTotal = 500, Standard = 50 },
            new ItemConfigurationMessage { Timestamp = DateTime.UtcNow, Quantity = 2, ItemId = 2, SetpointTotal = 600, Standard = 60 }
        };
        var mess = new ConfigurationMessage
        {
            MachineId = "LR1",
            DesktopAppMessage = configs
        };
        await endpoint.Send<ConfigurationMessage>(mess);
        Console.WriteLine("config");
        Console.ReadLine();
    }
}
finally
{
    Console.WriteLine("Stop...");
    await busControl.StopAsync();
}