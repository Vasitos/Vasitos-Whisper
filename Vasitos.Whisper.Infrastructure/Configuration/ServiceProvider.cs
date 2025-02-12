using Microsoft.Extensions.DependencyInjection;
using Vasitos.Whisper.Domain.Audio;
using Vasitos.Whisper.Infrastructure.Dispatchers.Audio;
using Vasitos.Whisper.Infrastructure.EventDriven;
using Vasitos.Whisper.Infrastructure.FileValidator;

namespace Vasitos.Whisper.Infrastructure.Configuration;

public static class ServiceProvider
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<IAudioPreProcessor, AudioPreProcessor.AudioPreProcessor>()
            .AddScoped<IAudioProcessor, AudioProcessor.AudioProcessor>()
            .AddScoped<IAudioProcessedDispatcher, AudioProcessedDispatcher>()
            .AddSingleton<IPublisher, Publisher>()
            .AddSingleton<IFileValidator, FileValidator.FileValidator>();
    }
}