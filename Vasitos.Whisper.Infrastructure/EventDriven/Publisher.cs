using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Vasitos.Whisper.Infrastructure.EventDriven;

public class Publisher(
    IConnectionMultiplexer redis,
    ILogger<Publisher> logger
) : IPublisher
{
    public async Task<long> PublishAsync<T>(T message, string topicName)
    {
        var pub = redis.GetSubscriber();
        var json = JsonSerializer.Serialize(message);
        logger.LogInformation("Publishing message to topic {topicName}: {json}", topicName, json);
        var offset = await pub.PublishAsync(topicName, json, CommandFlags.FireAndForget);
        logger.LogInformation("Message published to topic {topicName} with offset {offset}", topicName, offset);
        return offset;
    }
}