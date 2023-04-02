var busControl = Bus.Factory.CreateUsingGrpc(x =>
{
    x.Host(h =>
    {
        h.Host = "127.0.0.1";
        h.Port = 8182;

        h.AddServer(new Uri("http://127.0.0.1:8181"));
    });

    x.ReceiveEndpoint("event-listener", e =>
    {
        e.Consumer<CycleMessageConsumer>();
        e.Consumer<MachineMessageConsumer>();
        e.Consumer<FeedbackMessageConsumer>();
        e.Consumer<UaDoubleDataConsumer>();
        e.Consumer<UaBooleanDataConsumer>();
        e.Consumer<UaIntegerDataConsumer>();
    });
});

var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

await busControl.StartAsync(source.Token);

try
{
    Console.WriteLine("Dang nge ne");
    
    while (true)
    {
        string? key = Console.ReadLine();
        var endpoint = await busControl.GetSendEndpoint(new Uri("http://127.0.0.1:8181/send-config"));

        if (key == "com")
        {
            await endpoint.Send<CommandMessage>(new CommandMessage
            {
                MachineId = "L6",
                Timestamp = DateTime.UtcNow,
                Command = ECommand.ChangeMoldDone
            });
        }
        else if (key == "cfg")
        {
            await endpoint.Send<ConfigurationMessage>(new ConfigurationMessage
            {
                MachineId = "L6",
                Timestamp = DateTime.UtcNow,
                MoldId = "HA30",
                ProductId = "HA30",
                CycleTime = 120
            });
        }
        else if (key == "syc")
        {
            await endpoint.Send<SychronizeTimeMessage>(new SychronizeTimeMessage
            {
                MachineId = "L6",
                Year = 2022,
                Month = 05,
                Day = 07,
                Hour = 10,
                Min = 30,
                Sec = 30
            });
        }
        else if (key == "down")
        {
            await endpoint.Send<FileDownloadMessage>(new FileDownloadMessage
            {
                DownloadLink = "http://192.168.1.213/S2100522.CSV",
                FilePath = @"E:\SISTRAIN\27_WEBSCADA_CHA\000_LVTN\WorkerService\Data\ca1_10_05_2022.csv"
            });
        }
        Console.WriteLine("send");
    }
}
finally
{
    await busControl.StopAsync();
}
