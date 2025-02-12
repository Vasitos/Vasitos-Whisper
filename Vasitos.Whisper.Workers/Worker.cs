using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Vasitos.Whisper.Application.AudioProcessorUseCase;
using Vasitos.Whisper.Domain.Audio.Events;
using Vasitos.Whisper.Infrastructure.EventDriven;
using Vasitos.Whisper.Infrastructure.EventDriven.Configuration;

namespace Vasitos.Whisper.Workers;

public class Worker(
    ILogger<Worker> logger,
    IConnectionMultiplexer redis,
    IOptions<QueueConfiguration> queueConfiguration,
    IPublisher publisher,
    IServiceProvider serviceProvider)
    : BackgroundService
{
    private readonly string _audioQueue = queueConfiguration.Value.AudioQueue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        var subscriber = redis.GetSubscriber();
        logger.LogInformation("Subscribing to {Queue}", _audioQueue);
        await subscriber.SubscribeAsync(_audioQueue, async void (channel, message) =>
        {
            AudioDto? audioDto = null;
            try
            {
                if (!message.HasValue)
                {
                    logger.LogWarning("Event from channel {Channel} does not contain any message value", channel);
                    return;
                }

                audioDto = JsonSerializer.Deserialize<AudioDto>(message!);
                if (audioDto != null)
                {
                    logger.LogInformation(
                        "Received message on channel {Channel}: {@AudioDto}",
                        channel,
                        audioDto);
                    // Process the audioDto object as needed
                    using var scope = serviceProvider.CreateScope();
                    var useCase = scope.ServiceProvider.GetRequiredService<IAudioProcessorUseCase>();

                    // Execute the use case
                    await useCase.ExecuteAsync(audioDto);
                }
                else
                {
                    logger.LogWarning("Deserialized AudioDto is null for message: {Message}", message);
                }
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to deserialize message: {Message}", message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process message: {Message}", message);
                if (audioDto is not null)
                {
                    var rejectedMessage = new AudioRejectedDto
                    {
                        Id = audioDto.Id,
                        ChannelId = audioDto.ChannelId,
                        User = audioDto.User,
                        UserId = audioDto.UserId,
                        GuildId = audioDto.GuildId,
                        Reason = "Skill issue because gato and daves does not know how to code"
                    };
                    await publisher.PublishAsync(rejectedMessage, queueConfiguration.Value.RejectedAudioQueue);
                }
            }
        });
    }
}