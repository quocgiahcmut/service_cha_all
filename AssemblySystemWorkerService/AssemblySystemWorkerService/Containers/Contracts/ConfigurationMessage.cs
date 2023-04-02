namespace AssemblySystemServiceContainers;

public class ConfigurationMessage
{
    public string MachineId { get; set; }
    public List<ItemConfigurationMessage> DesktopAppMessage { get; set; }
}

public class ItemConfigurationMessage
{
    public DateTime Timestamp { get; set; }
    public int Quantity { get; set; }
    public int ItemId { get; set; }
    public int SetpointTotal { get; set; }
    public int Standard { get; set; }
}
