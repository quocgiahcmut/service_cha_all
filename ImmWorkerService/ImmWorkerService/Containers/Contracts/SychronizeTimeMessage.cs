namespace ImmServiceContainers;

public class SychronizeTimeMessage
{
    public string MachineId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public int Hour { get; set; }
    public int Min { get; set; }
    public int Sec { get; set; }
}

public class SychronizeTimeMqttMessage
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public int Hour { get; set; }
    public int Min { get; set; }
    public int Sec { get; set; }
}
