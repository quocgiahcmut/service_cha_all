IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {
        var config = builder.Configuration;

        services.Configure<MqttClientHelperOptions>(config.GetSection("MqttClientHelperOptions"));
        services.AddSingleton<MqttClientHelper>();
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
                cfg.Host(h =>
                {
                    h.Host = "127.0.0.1";
                    h.Port = 8181;
                });

                cfg.ReceiveEndpoint("send-config", e =>
                {
                    e.Consumer(() => new ConfigurationMessageConsumer(serviceProvider.GetRequiredService<MqttClientHelper>()));
                });
            });
        });
    })
    .Build();

await host.RunAsync();
