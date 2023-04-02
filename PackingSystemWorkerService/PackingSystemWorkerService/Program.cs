IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {
        var config = builder.Configuration;

        services.Configure<TcpServerHelperOptions>(config.GetSection("TcpServerHelperOptions"));
        services.AddSingleton<TcpServerHelper>();
        services.Configure<MqttClientHelperOptions>(config.GetSection("MqttClientHelperOptions"));
        services.AddSingleton<MqttClientHelper>();
        services.Configure<SparkplugClientHelperOptions>(config.GetSection("SparkplugClientHelperOptions"));
        services.AddSingleton<SparkplugClientHelper>();

        var serviceProvider = services.BuildServiceProvider();

        services.AddHostedService<TcpWorker>(provider =>
        {
            var tw = new TcpWorker(
                provider.GetRequiredService<ILogger<TcpWorker>>(),
                provider.GetRequiredService<IBusControl>(),
                serviceProvider.GetRequiredService<TcpServerHelper>(),
                serviceProvider.GetRequiredService<SparkplugClientHelper>());

            return tw;
        });
        #region MqattWorker
        /*services.AddHostedService<MqttWorker>(provider =>
        {
            var mw = new MqttWorker(
                provider.GetRequiredService<ILogger<MqttWorker>>(),
                provider.GetRequiredService<IBusControl>(),
                serviceProvider.GetRequiredService<MqttClientHelper>(),
                serviceProvider.GetRequiredService<SparkplugClientHelper>());

            return mw;
        });*/
        #endregion
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
                    e.Consumer(() => new RasConfigurationMessageConsumer(serviceProvider.GetRequiredService<TcpServerHelper>()));
                    e.Consumer(() => new EspConfigurationMessageConsumer(serviceProvider.GetRequiredService<MqttClientHelper>()));
                });
            });
        });
    })
    .Build();

await host.RunAsync();
