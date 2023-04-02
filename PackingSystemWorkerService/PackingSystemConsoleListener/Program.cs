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

    while (true)
    {
        Console.ReadLine();

        var endpoint = await busControl.GetSendEndpoint(new Uri("http://127.0.0.1:8181/send-config"));

        /*List<ItemConfigurationMessage> configs = new List<ItemConfigurationMessage>
        {
            new ItemConfigurationMessage { Timestamp = DateTime.UtcNow, Quantity = 2, ItemId = 1, ProductMass = 0.125, CompletedProduct = 120, ErrorProduct = 12 },
            new ItemConfigurationMessage { Timestamp = DateTime.UtcNow, Quantity = 2, ItemId = 2, ProductMass = 0.452, CompletedProduct = 150, ErrorProduct = 18 }
        };
        var mess = new EspConfigurationMessage
        {
            MachineId = "DG2",
            DesktopAppMessage = configs
        };
        await endpoint.Send<EspConfigurationMessage>(mess);*/

        List<string> boms = new List<string> { "tp1", "tp2", "tp3" };
        List<ItemRasConfigurationMessage> items = new List<ItemRasConfigurationMessage>
        {
            new ItemRasConfigurationMessage { SetpointTotal = 120, ProductId = "sp1", ProductName = "sanpham1", Boms = boms, ProductMass = 0.125, Standard = "huhu", CompletedProduct = 0, ErrorProduct = 0 },
            new ItemRasConfigurationMessage { SetpointTotal = 120, ProductId = "sp2", ProductName = "sanpham2", Boms = boms, ProductMass = 0.145, Standard = "hihi", CompletedProduct = 0, ErrorProduct = 0 }
        };
        RasConfigurationMessage mess = new RasConfigurationMessage { Timestamp = DateTime.UtcNow, Quantity = 2, Items = items };

        await endpoint.Send<RasConfigurationMessage>(mess);
        Console.WriteLine("config");
    }
}
finally
{
    Console.WriteLine("Stop...");
    await busControl.StopAsync();
}