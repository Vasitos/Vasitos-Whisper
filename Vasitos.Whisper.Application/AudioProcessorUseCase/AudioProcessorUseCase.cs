using Vasitos.Whisper.Domain.Audio;
using Vasitos.Whisper.Domain.Audio.Events;

namespace Vasitos.Whisper.Application.AudioProcessorUseCase;

public class AudioProcessorUseCase(
    IAudioPreProcessor audioPreProcessor,
    IAudioProcessor audioProcessor,
    IAudioProcessedDispatcher audioProcessedDispatcher)
    : IAudioProcessorUseCase
{
    public async Task ExecuteAsync(AudioDto audioDto)
    {
        var audio = Audio.BuildFromDto(audioDto);
        var preProcessedAudio = audioPreProcessor.PreProcess(audio);
        audio.PreProcessedAudioPath = preProcessedAudio;
        var result = await audioProcessor.ProcessAsync(audio);
        audio.TranscodedAudioPath = result;
        await audioProcessedDispatcher.DispatchAsync(audio);
    }
}