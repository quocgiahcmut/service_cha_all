namespace ImmWorkerService.WorkerServices;

public class OpcUaWorker : BackgroundService
{
    private readonly ILogger<OpcUaWorker> _logger;
    private readonly IBusControl _busControl;
    private readonly LargeOneUaClientHelper _largeOneUaClient;
    private readonly LargeTwoUaClientHelper _largeTwoUaClient;
    private readonly LargeThreeUaClientHelper _largeThreeUaClient;

    public OpcUaWorker(ILogger<OpcUaWorker> logger, IBusControl busControl, LargeOneUaClientHelper largeOneUaClient, LargeTwoUaClientHelper largeTwoUaClient, LargeThreeUaClientHelper largeThreeUaClient)
    {
        _logger = logger;
        _busControl = busControl;
        _largeOneUaClient = largeOneUaClient;
        _largeTwoUaClient = largeTwoUaClient;
        _largeThreeUaClient = largeThreeUaClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var uaOneConnected = await _largeOneUaClient.ConnectAsync();
        _logger.LogInformation("Connected UaOneServer: {1}", uaOneConnected);
        var uaTwoConnected = await _largeTwoUaClient.ConnectAsync();
        _logger.LogInformation("Connected UaTwoServer: {1}", uaTwoConnected);
        var uaThreeConnected = await _largeThreeUaClient.ConnectAsync();
        _logger.LogInformation("Connected UaThreeServer: {1}", uaThreeConnected);

        var subOne = _largeOneUaClient.Subscribe(1000);
        var subTwo = _largeTwoUaClient.Subscribe(1000);
        var subThree = _largeThreeUaClient.Subscribe(1000);

        #region L2
        //real
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"rMotorTemp\"", "MotorTemp", 1000, On_L2_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"rOilTemp\"", "OilTemp", 1000, On_L2_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"rTempZone1\"", "TempZone1", 1000, On_L2_MonitoredItemNotification);
        //int
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"diLastCycle\"", "LastCycle", 1000, On_L2_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"diLastCoolingTime\"", "LastCoolingTime", 1000, On_L2_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"diCounterShot\"", "CounterShot", 1000, On_L2_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"diCounterShotToday\"", "CounterShotToday", 1000, On_L2_MonitoredItemNotification);
        //bool
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"bAuto\"", "Auto", 1000, On_L2_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"bClampMold\"", "ClampMold", 1000, On_L2_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"bGreenAlarm\"", "GreenAlarm", 1000, On_L2_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"bHalfAuto\"", "HalfAuto", 1000, On_L2_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"bManual\"", "Manual", 1000, On_L2_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"bRedAlarm\"", "RedAlarm", 1000, On_L2_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"bSafetyDoor\"", "SafetyDoor", 1000, On_L2_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"bSetup\"", "Setup", 1000, On_L2_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subOne, "ns=3;s=\"L2\".\"bYellowAlarm\"", "YellowAlarm", 1000, On_L2_MonitoredItemNotification);
        #endregion

        #region L5
        //real
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"rMotorTemp\"", "MotorTemp", 1000, On_L5_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"rOilTemp\"", "OilTemp", 1000, On_L5_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"rTempZone1\"", "TempZone1", 1000, On_L5_MonitoredItemNotification);
        //int
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"diLastCycle\"", "LastCycle", 1000, On_L5_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"diLastCoolingTime\"", "LastCoolingTime", 1000, On_L5_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"diCounterShot\"", "CounterShot", 1000, On_L5_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"diCounterShotToday\"", "CounterShotToday", 1000, On_L5_MonitoredItemNotification);
        //bool
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"bAuto\"", "Auto", 1000, On_L5_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"bClampMold\"", "ClampMold", 1000, On_L5_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"bGreenAlarm\"", "GreenAlarm", 1000, On_L5_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"bHalfAuto\"", "HalfAuto", 1000, On_L5_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"bManual\"", "Manual", 1000, On_L5_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"bRedAlarm\"", "RedAlarm", 1000, On_L5_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"bSafetyDoor\"", "SafetyDoor", 1000, On_L5_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"bSetup\"", "Setup", 1000, On_L5_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subTwo, "ns=3;s=\"L5\".\"bYellowAlarm\"", "YellowAlarm", 1000, On_L5_MonitoredItemNotification);
        #endregion

        #region L12
        //real
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"rMotorTemp\"", "MotorTemp", 1000, On_L12_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"rOilTemp\"", "OilTemp", 1000, On_L12_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"rTempZone1\"", "TempZone1", 1000, On_L12_MonitoredItemNotification);
        //int
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"diLastCycle\"", "LastCycle", 1000, On_L12_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"diLastCoolingTime\"", "LastCoolingTime", 1000, On_L12_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"diCounterShot\"", "CounterShot", 1000, On_L12_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"diCounterShotToday\"", "CounterShotToday", 1000, On_L12_MonitoredItemNotification);
        //bool
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"bAuto\"", "Auto", 1000, On_L12_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"bClampMold\"", "ClampMold", 1000, On_L12_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"bGreenAlarm\"", "GreenAlarm", 1000, On_L12_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"bHalfAuto\"", "HalfAuto", 1000, On_L12_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"bManual\"", "Manual", 1000, On_L12_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"bRedAlarm\"", "RedAlarm", 1000, On_L12_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"bSafetyDoor\"", "SafetyDoor", 1000, On_L12_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"bSetup\"", "Setup", 1000, On_L12_MonitoredItemNotification);
        _largeOneUaClient.AddMonitoredItem(subThree, "ns=3;s=\"L12\".\"bYellowAlarm\"", "YellowAlarm", 1000, On_L12_MonitoredItemNotification);
        #endregion
    }

    void On_L2_MonitoredItemNotification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
        Console.WriteLine("Notification Received for Variable \"{0}\" and Value = {1}.", monitoredItem.DisplayName, notification.Value);
        string name = "L2." + monitoredItem.DisplayName;

        switch (Type.GetTypeCode(notification.Value.Value.GetType()))
        {
            case TypeCode.Single:
                {
                    double value = Math.Round(Convert.ToSingle(notification.Value.Value), 3);
                    var data = new UaDoubleData { Name = name, Value = value };
                    _busControl.Publish<UaDoubleData>(data);
                }
                break;
            case TypeCode.Boolean:
                {
                    bool value = Convert.ToBoolean(notification.Value.Value);
                    var data = new UaBooleanData { Name = name, Value = value };
                    _busControl.Publish<UaBooleanData>(data);
                }
                break;
            case TypeCode.Int32:
                {
                    int value = Convert.ToInt32(notification.Value.Value);
                    var data = new UaIntegerData { Name = name, Value = value };
                        _busControl.Publish<UaIntegerData>(data);
                }
                break;
        }
    }

    void On_L5_MonitoredItemNotification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
        Console.WriteLine("Notification Received for Variable \"{0}\" and Value = {1}.", monitoredItem.DisplayName, notification.Value);
        string name = "L5." + monitoredItem.DisplayName;

        switch (Type.GetTypeCode(notification.Value.Value.GetType()))
        {
            case TypeCode.Single:
                {
                    double value = Math.Round(Convert.ToSingle(notification.Value.Value), 3);
                    var data = new UaDoubleData { Name = name, Value = value };
                    _busControl.Publish<UaDoubleData>(data);
                }
                break;
            case TypeCode.Boolean:
                {
                    bool value = Convert.ToBoolean(notification.Value.Value);
                    var data = new UaBooleanData { Name = name, Value = value };
                    _busControl.Publish<UaBooleanData>(data);
                }
                break;
            case TypeCode.Int32:
                {
                    int value = Convert.ToInt32(notification.Value.Value);
                    var data = new UaIntegerData { Name = name, Value = value };
                    _busControl.Publish<UaIntegerData>(data);
                }
                break;
        }
    }

    void On_L12_MonitoredItemNotification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
        Console.WriteLine("Notification Received for Variable \"{0}\" and Value = {1}.", monitoredItem.DisplayName, notification.Value);
        string name = "L12." + monitoredItem.DisplayName;

        switch (Type.GetTypeCode(notification.Value.Value.GetType()))
        {
            case TypeCode.Single:
                {
                    double value = Math.Round(Convert.ToSingle(notification.Value.Value), 3);
                    var data = new UaDoubleData { Name = name, Value = value };
                    _busControl.Publish<UaDoubleData>(data);
                }
                break;
            case TypeCode.Boolean:
                {
                    bool value = Convert.ToBoolean(notification.Value.Value);
                    var data = new UaBooleanData { Name = name, Value = value };
                    _busControl.Publish<UaBooleanData>(data);
                }
                break;
            case TypeCode.Int32:
                {
                    int value = Convert.ToInt32(notification.Value.Value);
                    var data = new UaIntegerData { Name = name, Value = value };
                    _busControl.Publish<UaIntegerData>(data);
                }
                break;
        }
    }
}
