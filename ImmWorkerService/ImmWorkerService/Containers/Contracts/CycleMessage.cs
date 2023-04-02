namespace ImmServiceContainers;

public class CycleMessage
{
    public string MachineId { get; set; }
    public DateTime Timestamp { get; set; }
    public double CycleTime { get; set; }
    public double OpenTime { get; set; }
    public int Mode { get; set; }
    public int CounterShot { get; set; }
    public string MoldId { get; set; }
    public string ProductId { get; set; }
    public double SetCycle { get; set; }
}

public class CycleMqttMessage
{
    public DateTime Timestamp { get; set; }
    public double CycleTime { get; set; }
    public double OpenTime { get; set; }
    public int Mode { get; set; }
    public int CounterShot { get; set; }
    public string MoldId { get; set; }
    public string ProductId { get; set; }
    public double SetCycle { get; set; }
}
