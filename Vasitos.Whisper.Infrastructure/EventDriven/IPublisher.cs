namespace Vasitos.Whisper.Infrastructure.EventDriven;

public interface IPublisher
{
    public Task<long> PublishAsync<T>(T message, string topicName);
}