namespace Vasitos.Whisper.Infrastructure.EventDriven.Configuration;

public class QueueConfiguration
{
    public required string AudioQueue { get; set; }
    public required string ProcessedAudioQueue { get; set; }
    public required string RejectedAudioQueue { get; set; }
}