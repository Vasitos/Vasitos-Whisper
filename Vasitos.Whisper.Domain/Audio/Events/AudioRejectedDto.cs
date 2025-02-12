using System.Text.Json.Serialization;

namespace Vasitos.Whisper.Domain.Audio.Events;

public class AudioRejectedDto
{
    [JsonPropertyName("id")] public required Guid Id { get; set; }
    [JsonPropertyName("user")] public required string User { get; set; }
    [JsonPropertyName("userId")] public required long UserId { get; set; }
    [JsonPropertyName("channelId")] public required long ChannelId { get; set; }
    [JsonPropertyName("guildId")] public required long GuildId { get; set; }
    [JsonPropertyName("reason")] public required string Reason { get; set; }

}