namespace AssemblySystemServiceContainers;

public class MachineMessage
{
    #pragma warning disable CS8618
    public string MachineId { get; set; }
    public DateTime Timestamp { get; set; }
    public EMachineStatus MachineStatus { get; set; }
    #pragma warning restore CS8618
}

public class MachineMqttMessage
{
    public DateTime Timestamp { get; set; }
    public EMachineStatus MachineStatus { get; set; }
}
