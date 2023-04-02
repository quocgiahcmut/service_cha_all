using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Subscribing;
using MQTTnet.Internal;
using MQTTnet.Exceptions;

using Microsoft.Extensions.Options;

namespace ImmWorkerService.ProtocolServices.MqttCore;

public class MqttClientHelper
{
    IMqttClient? _mqttClient;

    public bool IsConnected => _mqttClient?.IsConnected == true;
    public MqttClientHelperOptions Options { get; set; }

    public Func<MqttApplicationMessageReceivedEventArgs, Task> ApplicationMessageReceivedEvent;

    #pragma warning disable CS8618
    public MqttClientHelper(MqttClientHelperOptions options)
    {
        Options = options;
    }

    public MqttClientHelper(IOptions<MqttClientHelperOptions> options)
    {
        Options = options.Value;
    }
    #pragma warning restore CS8618

    public async Task<MqttClientConnectResult> ConnectAsync()
    {
        if (_mqttClient != null)
        {
            await _mqttClient.DisconnectAsync();
            _mqttClient.Dispose();
        }

        _mqttClient = new MqttFactory().CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(Options.Host, Options.Port)
            .WithCommunicationTimeout(TimeSpan.FromSeconds(Options.CommunicationTimeout))
            .WithClientId(Options.ClientId)
            .WithCredentials(Options.UserName, Options.Password)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(Options.KeepAliveInterval)); //Thong baos t chet r

        _mqttClient.UseApplicationMessageReceivedHandler(OnApplicationMessageReceived);
        //_mqttClient.UseDisconnectedHandler(OnDisconnected);

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(Options.CommunicationTimeout));

        try
        {
            return await _mqttClient.ConnectAsync(mqttClientOptions.Build(), timeout.Token);
        }
        catch (OperationCanceledException)
        {
            if (timeout.IsCancellationRequested)
            {
                throw new MqttCommunicationTimedOutException();
            }

            throw;
        }
    }

    public async Task DisconnectAsync()
    {
        await _mqttClient.DisconnectAsync();
    }

    public async Task<MqttClientSubscribeResult> Subscribe(string topic)
    {
        var topicFilter = new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .Build();

        var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(topicFilter)
            .Build();

        var subscribeResult = await _mqttClient.SubscribeAsync(subscribeOptions);

        return subscribeResult;
    }

    public async Task<MqttClientPublishResult> Publish(string topic, string payload, bool retainFlag)
    {
        var applicationMessageBuilder = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithRetainFlag(retainFlag)
            .WithPayload(payload);

        var applicationMessage = applicationMessageBuilder.Build();

        return await _mqttClient.PublishAsync(applicationMessage);
    }

    Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        return ApplicationMessageReceivedEvent.Invoke(eventArgs);
    }

    Task OnDisconnected(MqttClientDisconnectedEventArgs arg)
    {
        if (arg.ClientWasConnected)
        {
            //arg.Reason
        }

        return Task.CompletedTask;
    }
}
