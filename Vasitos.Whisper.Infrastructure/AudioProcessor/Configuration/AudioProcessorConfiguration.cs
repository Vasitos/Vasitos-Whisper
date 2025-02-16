using Whisper.net.Ggml;
using Whisper.net.LibraryLoader;

namespace Vasitos.Whisper.Infrastructure.AudioProcessor.Configuration;

public class AudioProcessorConfiguration
{
    public required string ModelPath { get; set; }
    public required string Language { get; set; }
    public required string OutputPath { get; set; }
    public required int? Threads { get; set; }
    public required float? Temperature { get; set; }
    public GgmlType ModelType { get; set; } = GgmlType.Base;
    // Order matters
    public required RuntimeLibrary[] RuntimeLibraryOrder { get; set; }
    public bool UseContext { get; set; }
}
