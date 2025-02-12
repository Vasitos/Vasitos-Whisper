namespace Vasitos.Whisper.Domain.Audio;

public interface IAudioProcessor
{
    public Task<string> ProcessAsync(Audio audio);
}