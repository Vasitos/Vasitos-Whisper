using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vasitos.Whisper.Domain.Audio;
using Vasitos.Whisper.Domain.Audio.Events;
using Vasitos.Whisper.Infrastructure.EventDriven;
using Vasitos.Whisper.Infrastructure.EventDriven.Configuration;

namespace Vasitos.Whisper.Infrastructure.Dispatchers.Audio;

public class AudioProcessedDispatcher(
    IPublisher publisher,
    IOptions<QueueConfiguration> queueConfiguration,
    ILogger<AudioProcessedDispatcher> logger
) : IAudioProcessedDispatcher
{
    public async Task DispatchAsync(Domain.Audio.Audio audio)
    {
        logger.LogInformation("Dispatching audio {audio.Id} to processed queue", audio.ChannelId);
        var transcodedAudioPath = audio.TranscodedAudioPath;
        if (transcodedAudioPath is null) throw new ArgumentNullException(nameof(transcodedAudioPath));
        var processedAudio = new AudioProcessedDto
        {
            Id = audio.Id,
            ChannelId = audio.ChannelId,
            TranscodedAudioPath = transcodedAudioPath,
            User = audio.User,
            UserId = audio.UserId,
            GuildId = audio.GuildId
        };
        await publisher.PublishAsync(processedAudio, queueConfiguration.Value.ProcessedAudioQueue);
        logger.LogInformation("Audio {audio.Id} sent to the different data sources", audio.ChannelId);
    }
}