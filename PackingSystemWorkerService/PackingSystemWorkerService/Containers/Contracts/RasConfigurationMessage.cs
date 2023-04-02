namespace PackingSystemServiceContainers;

public class RasConfigurationMessage
{
    #pragma warning disable CS8618
    public string MachineId { get; set; }
    public DateTime Timestamp { get; set; }
    public int Quantity { get; set; }
    public List<ItemRasConfigurationMessage> Items { get; set; }
    #pragma warning disable CS8618
}

public class ItemRasConfigurationMessage
{
    public int SetpointTotal { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public List<string> Boms { get; set; }
    public double ProductMass { get; set; }
    public string Standard { get; set; }
    public int CompletedProduct { get; set; }
    public int ErrorProduct { get; set; }
}
