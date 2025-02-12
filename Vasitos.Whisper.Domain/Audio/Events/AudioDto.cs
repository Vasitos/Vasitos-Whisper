using System.Text.Json.Serialization;

namespace Vasitos.Whisper.Domain.Audio.Events;

public class AudioDto
{
    [JsonPropertyName("id")] public required Guid Id { get; set; }
    [JsonPropertyName("user")] public required string User { get; set; }
    [JsonPropertyName("userId")] public required long UserId { get; set; }
    [JsonPropertyName("guildId")] public required long GuildId { get; set; }
    [JsonPropertyName("path")] public required string Path { get; set; }
    [JsonPropertyName("channelId")] public required long ChannelId { get; set; }

    [JsonPropertyName("retryAttempts")] public int? RetryAttempts { get; set; }
}
