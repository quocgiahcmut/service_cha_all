namespace PackingSystemServiceContainers;

public class EspConfigurationMessage
{
    #pragma warning disable CS8618
    public string MachineId { get; set; }
    public List<ItemConfigurationMessage> DesktopAppMessage { get; set; }
    #pragma warning restore CS8618
}

public class ItemConfigurationMessage
{
    public DateTime Timestamp { get; set; }
    public int Quantity { get; set; }
    public int ItemId { get; set; }
    public double ProductMass { get; set; }
    public int CompletedProduct { get; set; }
    public int ErrorProduct { get; set; }
}
