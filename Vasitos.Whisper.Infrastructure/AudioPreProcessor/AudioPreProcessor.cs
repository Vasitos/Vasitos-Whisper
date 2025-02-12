using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SileroVad;
using Vasitos.Whisper.Domain.Audio;
using Vasitos.Whisper.Infrastructure.AudioPreProcessor.Configuration;
using Vasitos.Whisper.Infrastructure.FileValidator;

namespace Vasitos.Whisper.Infrastructure.AudioPreProcessor;

public class AudioPreProcessor(
    IFileValidator fileValidator,
    IOptions<AudioPreProcessorConfiguration> configuration,
    ILogger<AudioPreProcessor> logger)
    : IAudioPreProcessor
{
    private readonly AudioPreProcessorConfiguration _configuration = configuration.Value;
    private readonly Vad _vad = new();

    public string PreProcess(Audio audio)
    {
        logger.LogInformation("Preprocessing audio");
        var absolutePath = Path.Combine(_configuration.AudioPath, audio.Path);
        var validatedPath = fileValidator.ValidateFileExists(absolutePath);
        var ext = Path.GetExtension(validatedPath).ToLower();
        WaveStream waveFileReader = ext switch
        {
            ".wav" => new WaveFileReader(validatedPath),
            ".mp3" => new Mp3FileReader(validatedPath),
            _ => throw new NotSupportedException($"not supported {ext}")
        };
        var fileTrim = Path.ChangeExtension(validatedPath, "speech") + ".wav";
        if (fileValidator.FileExists(fileTrim))
        {
            logger.LogInformation("File {File} already exists. Deleting file to process it again", fileTrim);
            fileValidator.DeleteFile(fileTrim);
        }

        var totalTime = waveFileReader.TotalTime;
        var sampleProvider = waveFileReader.WaveFormat.SampleRate != _configuration.SampleRate
            ? new WdlResamplingSampleProvider(waveFileReader.ToSampleProvider(), _configuration.SampleRate).ToMono()
            : waveFileReader.ToSampleProvider();

        var array = new float[CountSamples(totalTime)];

        sampleProvider.Read(array, 0, array.Length);

        var resul = _vad.GetSpeechTimestamps(array, min_silence_duration_ms: 500, threshold: 0.5f);

        var audioSpeech = VadHelper.GetSpeechSamples(array, resul);

        using var fileWriter = new WaveFileWriter(fileTrim, new WaveFormat(_configuration.SampleRate, 1));
        foreach (var sample in audioSpeech) fileWriter.WriteSample(sample);
        fileWriter.Flush();
        waveFileReader.Dispose();
        logger.LogInformation("Finished processing audio, saved on {Path}", fileTrim);
        return fileTrim;
    }

    private static int CountSamples(TimeSpan time)
    {
        var waveFormat = new WaveFormat(16000, 1);

        return TimeSpanToSamples(time, waveFormat);
    }

    private static int TimeSpanToSamples(TimeSpan time, WaveFormat waveFormat)
    {
        return (int)(time.TotalSeconds * waveFormat.SampleRate) * waveFormat.Channels;
    }
}