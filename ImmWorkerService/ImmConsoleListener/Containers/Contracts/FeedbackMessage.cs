namespace ImmServiceContainers;

public class FeedbackMessage
{
    public string MachineId { get; set; }
    public EFeedback Mess { get; set; }
}

public class FeedbackMqttMessage
{
    public EFeedback Mess { get; set; }
}
