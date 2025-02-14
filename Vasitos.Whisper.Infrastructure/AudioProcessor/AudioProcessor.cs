using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vasitos.Whisper.Domain.Audio;
using Vasitos.Whisper.Infrastructure.AudioProcessor.Configuration;
using Vasitos.Whisper.Infrastructure.FileValidator;
using Whisper.net;
using Whisper.net.Ggml;
using Whisper.net.LibraryLoader;
using Whisper.net.Logger;

namespace Vasitos.Whisper.Infrastructure.AudioProcessor;

public class AudioProcessor(
    IFileValidator fileValidator,
    IOptions<AudioProcessorConfiguration> options,
    ILogger<AudioProcessor> logger) : IAudioProcessor
{
    private readonly AudioProcessorConfiguration _options = options.Value;

    public async Task<string> ProcessAsync(Audio audio)
    {
        RuntimeOptions.RuntimeLibraryOrder = [RuntimeLibrary.CpuNoAvx];
        logger.LogInformation("Transcoding audio");
        var parsedPath = fileValidator.ValidateFileExists(audio.PreProcessedAudioPath);
        if (!File.Exists(_options.ModelPath)) await DownloadModel(_options.ModelPath, _options.ModelType);
        fileValidator.EnsureDirectoryPathExists(_options.OutputPath);

        using var whisperLogger = LogProvider.AddConsoleLogging(WhisperLogLevel.Debug);
        using var whisperFactory = WhisperFactory.FromPath(_options.ModelPath);
        var builder = whisperFactory.CreateBuilder()
            .WithLanguage(_options.Language);
        if (_options.Threads.HasValue)
        {
            builder = builder.WithThreads(_options.Threads.Value);
        }

        if (!_options.UseContext)
        {
            builder = builder.WithNoContext();
        }
        
        await using var processor = builder.Build();
        await using var fileStream = File.OpenRead(parsedPath);

        logger.LogInformation("Start processing audio from {Audio}", parsedPath);

        var results = new List<string>();
        var sw = Stopwatch.StartNew();
        await foreach (var result in processor.ProcessAsync(fileStream))
            results.Add($"[{result.Start}->{result.End}]: {result.Text}");
        sw.Stop();
        logger.LogInformation("Processing took {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
        var outputFileName = GenerateOutputFileName(audio.UserId.ToString());
        var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        var outputFilePath = Path.Combine(_options.OutputPath, audio.User, date, outputFileName);
        fileValidator.EnsureDirectoryPathExists(outputFilePath);

        await File.WriteAllTextAsync(outputFilePath, string.Join(Environment.NewLine, results));

        logger.LogInformation("Audio successfully processed and saved to {OutputFilePath}", outputFilePath);
        return outputFilePath;
    }

    private async Task DownloadModel(string fileName, GgmlType ggmlType)
    {
        logger.LogInformation("Downloading Model {ModelPath}", fileName);
        await using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(ggmlType);
        await using var fileWriter = File.OpenWrite(fileName);
        await modelStream.CopyToAsync(fileWriter);
    }

    private static string GenerateOutputFileName(string userId)
    {
        var guid = Guid.NewGuid().ToString();
        var timestamp = DateTime.Now.ToString("dd-MM-yy HH:mm", CultureInfo.InvariantCulture);
        return $"{userId}-{guid}-{timestamp}.txt";
    }
}