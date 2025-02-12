using System.Text.Json.Serialization;

namespace Vasitos.Whisper.Domain.Audio.Events;

public class AudioProcessedDto
{
    [JsonPropertyName("id")] public required Guid Id { get; set; }
    [JsonPropertyName("transcodedAudioPath")]
    public required string TranscodedAudioPath { get; set; }
    [JsonPropertyName("user")] public required string User { get; set; }
    [JsonPropertyName("userId")] public required long UserId { get; set; }
    [JsonPropertyName("guildId")] public required long GuildId { get; set; }
    [JsonPropertyName("channelId")] public required long ChannelId { get; set; }
}