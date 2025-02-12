namespace Vasitos.Whisper.Domain.Audio;

public interface IAudioPreProcessor
{
    public string PreProcess(Audio audio);
}