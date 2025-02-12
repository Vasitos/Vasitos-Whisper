using Vasitos.Whisper.Domain.Audio.Events;

namespace Vasitos.Whisper.Application.AudioProcessorUseCase;

public interface IAudioProcessorUseCase
{
    public Task ExecuteAsync(AudioDto audioDto);
}