using ImmWorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {
        var config = builder.Configuration;

        services.Configure<MqttClientHelperOptions>(config.GetSection("MqttClientHelperOptions"));
        services.AddSingleton<MqttClientHelper>();
        services.AddSingleton<LargeOneUaClientHelper>(provider => { return new LargeOneUaClientHelper("opc.tcp://192.168.2.182:4840"); });
        services.AddSingleton<LargeTwoUaClientHelper>(provider => { return new LargeTwoUaClientHelper("opc.tcp://192.168.2.185:4840"); });
        services.AddSingleton<LargeThreeUaClientHelper>(provider => { return new LargeThreeUaClientHelper("opc.tcp://192.168.2.186:4840"); });
        services.Configure<SparkplugClientHelperOptions>(config.GetSection("SparkplugClientHelperOptions"));
        services.AddSingleton<SparkplugClientHelper>();

        var serviceProvider = services.BuildServiceProvider();

        services.AddHostedService<MqttWorker>(provider =>
        {
            var mw = new MqttWorker(
                provider.GetRequiredService<ILogger<MqttWorker>>(),
                provider.GetRequiredService<IBusControl>(),
                serviceProvider.GetRequiredService<MqttClientHelper>(),
                serviceProvider.GetRequiredService<SparkplugClientHelper>());

            return mw;
        });
        /*services.AddHostedService<OpcUaWorker>(provider =>
        {
            var ow = new OpcUaWorker(
                provider.GetRequiredService<ILogger<OpcUaWorker>>(),
                provider.GetRequiredService<IBusControl>(),
                serviceProvider.GetRequiredService<LargeOneUaClientHelper>(),
                serviceProvider.GetRequiredService<LargeTwoUaClientHelper>(),
                serviceProvider.GetRequiredService<LargeThreeUaClientHelper>());

            return ow;
        });*/
        services.AddHostedService<SparkplugWorker>(provider =>
        {
            var sw = new SparkplugWorker(
                provider.GetRequiredService<ILogger<SparkplugWorker>>(),
                serviceProvider.GetRequiredService<SparkplugClientHelper>());

            return sw;
        });

        services.AddMassTransit(x =>
        {
            x.UsingGrpc((context, cfg) =>
            {
                x.AddConsumer<ConfigurationMessageConsumer>();

                cfg.Host(h =>
                {
                    h.Host = "127.0.0.1";
                    h.Port = 8181;
                });

                cfg.ReceiveEndpoint("send-config", e =>
                {
                    e.Consumer(() => new ConfigurationMessageConsumer(serviceProvider.GetRequiredService<MqttClientHelper>()));
                    e.Consumer(() => new CommandMessageConsumer(serviceProvider.GetRequiredService<MqttClientHelper>()));
                    e.Consumer(() => new SychronizeTimeMessageConsumer(serviceProvider.GetRequiredService<MqttClientHelper>()));
                    e.Consumer(() => new FileDownloadMessageConsumer());
                });
            });
        });
    })
    .Build();

await host.RunAsync();

