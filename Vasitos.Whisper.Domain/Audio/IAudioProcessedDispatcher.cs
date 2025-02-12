namespace Vasitos.Whisper.Domain.Audio;

public interface IAudioProcessedDispatcher
{
    public Task DispatchAsync(Audio audio);
}