using Whisper.net.Ggml;

namespace Vasitos.Whisper.Infrastructure.AudioProcessor.Configuration;

public class AudioProcessorConfiguration
{
    public required string ModelPath { get; set; }
    public required string Language { get; set; }
    public required string OutputPath { get; set; }
    public required int? Threads { get; set; }
    public GgmlType ModelType { get; set; } = GgmlType.Base;
    public bool UseContext { get; set; }
}