namespace ImmServiceContainers;

public class FeedbackMessageConsumer : IConsumer<FeedbackMessage>
{
    public async Task Consume(ConsumeContext<FeedbackMessage> context)
    {
        var message = context.Message;

        Console.WriteLine("Feedback: {0}", message.Mess);
    }
}
