using Vasitos.Whisper.Domain.Audio.Events;

namespace Vasitos.Whisper.Domain.Audio;

public class Audio
{
    public required Guid Id { get; set; }
    public required string User { get; set; }
    public required long UserId { get; set; }
    public required string Path { get; set; }
    public required long ChannelId { get; set; }
    public string? PreProcessedAudioPath { get; set; }
    public string? TranscodedAudioPath { get; set; }
    public required long GuildId { get; set; }

    public static Audio BuildFromDto(AudioDto dto)
    {
        return new Audio
        {
            Id = dto.Id,
            Path = dto.Path,
            User = dto.User,
            UserId = dto.UserId,
            ChannelId = dto.ChannelId,
            GuildId = dto.GuildId
        };
    }
}