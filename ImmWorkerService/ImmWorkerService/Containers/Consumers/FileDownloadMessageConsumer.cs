using System.Net;

namespace ImmServiceContainers;

public class FileDownloadMessageConsumer : IConsumer<FileDownloadMessage>
{
    public async Task Consume(ConsumeContext<FileDownloadMessage> context)
    {
        Console.WriteLine("FileDownloadMessageReceived");
        var message = context.Message;

        try
        {
            #pragma warning disable SYSLIB0014
            using (WebClient client = new WebClient())
            {
                await client.DownloadFileTaskAsync(message.DownloadLink, message.FilePath);
            }
            #pragma warning restore SYSLIB0014
        }
        catch (Exception ex)
        { Console.WriteLine(ex.Message); }
    }
}
