using Opc.Ua;
using Opc.Ua.Client;

using System.Collections;

namespace ImmWorkerService.ProtocolServices.UaCore;

public class UaClientHelper
{
    #region Properties

    public Session Session => _session;
    public string ServerUrl { get; set; }

    internal Session _session;
    internal ApplicationConfiguration _configuration;
    internal Action<IList, IList> _validateResponse;

    #endregion

    #region Constructor

    public UaClientHelper(string serverUrl)
    {
        ServerUrl = serverUrl;
        _validateResponse = ClientBase.ValidateResponse;
        _configuration = CreateClientConfiguration();
        _configuration.CertificateValidator.CertificateValidation += CertificateValidation;
    }

    #endregion

    #region Methods

    public async Task<bool> ConnectAsync()
    {
        try
        {
            if (_session != null && _session.Connected == true)
            { }
            else
            {
                EndpointDescription endpointDescription = CoreClientUtils.SelectEndpoint(ServerUrl, false);

                EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(_configuration);
                ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                Session session = await Session.Create(
                    _configuration,
                    endpoint,
                    false,
                    false,
                    _configuration.ApplicationName,
                    30 * 60 * 1000,
                    new UserIdentity(),
                    null
                );

                if (session != null && session.Connected == true)
                {
                    _session = session;
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool Disconnect()
    {
        try
        {
            if (_session != null)
            {
                _session.Close();
                _session.Dispose();
                _session = null;

                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    public Subscription Subscribe(int publishingInterval)
    {
        Subscription subscription = new Subscription(_session.DefaultSubscription);

        subscription.PublishingEnabled = true;
        subscription.PublishingInterval = publishingInterval;

        _session.AddSubscription(subscription);

        subscription.Create();

        return subscription;
    }

    public MonitoredItem AddMonitoredItem(Subscription subscription, string nodeId, string itemName, int samplingInterval, MonitoredItemNotificationEventHandler handler)
    {
        MonitoredItem item = new MonitoredItem(subscription.DefaultItem);
        item.DisplayName = itemName;
        item.StartNodeId = new NodeId(nodeId);
        item.AttributeId = Attributes.Value;
        item.SamplingInterval = samplingInterval;
        item.Notification += new MonitoredItemNotificationEventHandler(handler);

        subscription.AddItem(item);
        subscription.ApplyChanges();

        return item;
    }

    public MonitoredItemNotificationEventHandler ItemChangedNotification = null;
    private void Notification_MonitoredItem(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        ItemChangedNotification(monitoredItem, e);
    }

    private void OnMonitoredItemNotification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        try
        {
            // Log MonitoredItem Notification event
            MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
            Console.WriteLine("Notification Received for Variable \"{0}\" and Value = {1}.", monitoredItem.DisplayName, notification.Value);
        }
        catch (Exception ex)
        {
            Console.WriteLine("OnMonitoredItemNotification error: {0}", ex.Message);
        }
    }

    #endregion

    #region Certificate
    private void CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
    {
        bool certificateAccepted = true;

        ServiceResult error = e.Error;
        while (error != null)
        {
            error = error.InnerResult;
        }

        e.AcceptAll = certificateAccepted;
    }
    #endregion

    private static ApplicationConfiguration CreateClientConfiguration()
    {
        ApplicationConfiguration configuration = new ApplicationConfiguration();

        configuration.ApplicationName = "May Ep Client";
        configuration.ApplicationType = ApplicationType.Client;
        configuration.ApplicationUri = "urn:MayEpClient";
        configuration.ProductUri = "Cha.MayEpClient";

        configuration.SecurityConfiguration = new SecurityConfiguration();
        configuration.SecurityConfiguration.ApplicationCertificate = new CertificateIdentifier();
        configuration.SecurityConfiguration.ApplicationCertificate.StoreType = CertificateStoreType.Directory;
        configuration.SecurityConfiguration.ApplicationCertificate.StorePath = "CurrentUser\\My";
        configuration.SecurityConfiguration.ApplicationCertificate.SubjectName = configuration.ApplicationName;

        configuration.SecurityConfiguration.TrustedIssuerCertificates.StoreType = CertificateStoreType.Directory;
        configuration.SecurityConfiguration.TrustedIssuerCertificates.StorePath = "CurrentUser\\Root";

        configuration.SecurityConfiguration.TrustedPeerCertificates.StoreType = CertificateStoreType.Directory;
        configuration.SecurityConfiguration.TrustedPeerCertificates.StorePath = "CurrentUser\\Root";

        configuration.SecurityConfiguration.RejectedCertificateStore = new CertificateStoreIdentifier();
        configuration.SecurityConfiguration.RejectedCertificateStore.StoreType = CertificateStoreType.Directory;
        configuration.SecurityConfiguration.RejectedCertificateStore.StorePath = "CurrentUser\\Rejected";

        configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates = false;

        configuration.TransportQuotas = new TransportQuotas();
        configuration.TransportQuotas.OperationTimeout = 600000;
        configuration.TransportQuotas.MaxStringLength = 1048576;
        configuration.TransportQuotas.MaxByteStringLength = 1048576;
        configuration.TransportQuotas.MaxArrayLength = 65535;
        configuration.TransportQuotas.MaxMessageSize = 4194304;
        configuration.TransportQuotas.MaxBufferSize = 65535;
        configuration.TransportQuotas.ChannelLifetime = 300000;
        configuration.TransportQuotas.SecurityTokenLifetime = 3600000;

        configuration.ClientConfiguration = new ClientConfiguration();
        configuration.ClientConfiguration.DefaultSessionTimeout = 360000;

        configuration.Validate(ApplicationType.Client);

        return configuration;
    }
}
