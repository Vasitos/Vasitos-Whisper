namespace Vasitos.Whisper.Infrastructure.AudioPreProcessor.Configuration;

public class AudioPreProcessorConfiguration
{
    public string ModelPath { get; set; } = null!;
    public string AudioPath { get; set; } = null!;
    public int SampleRate { get; set; }
    public float Threshold { get; set; }
    public int MinSpeechDurationMs { get; set; }
    public float MaxSpeechDurationSeconds { get; set; } = float.PositiveInfinity;
    public int MinSilenceDurationMs { get; set; }
    public int SpeechPadMs { get; set; }
    public int SilencePadSeconds { get; set; }
}