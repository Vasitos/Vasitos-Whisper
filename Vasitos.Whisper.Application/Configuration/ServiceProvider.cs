using Microsoft.Extensions.DependencyInjection;
using Vasitos.Whisper.Application.AudioProcessorUseCase;

namespace Vasitos.Whisper.Application.Configuration;

public static class ServiceProvider
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddScoped<IAudioProcessorUseCase, AudioProcessorUseCase.AudioProcessorUseCase>();
    }
}