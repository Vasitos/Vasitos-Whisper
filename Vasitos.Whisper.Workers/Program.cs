using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Vasitos.Whisper.Application.Configuration;
using Vasitos.Whisper.Infrastructure.AudioPreProcessor.Configuration;
using Vasitos.Whisper.Infrastructure.AudioProcessor.Configuration;
using Vasitos.Whisper.Infrastructure.Configuration;
using Vasitos.Whisper.Infrastructure.EventDriven.Configuration;
using Vasitos.Whisper.Workers;
using Vasitos.Whisper.Workers.Configuration.Models;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<QueueConfiguration>(
    builder.Configuration.GetSection("QueueConfiguration"));
builder.Services.Configure<RedisCacheSettings>(
    builder.Configuration.GetSection("Cache"));
builder.Services.Configure<AudioPreProcessorConfiguration>(
    builder.Configuration.GetSection("AudioPreProcessorConfiguration"));
builder.Services.Configure<AudioProcessorConfiguration>(
    builder.Configuration.GetSection("AudioProcessorConfiguration"));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisSettings = sp.GetRequiredService<IOptions<RedisCacheSettings>>().Value;
    return ConnectionMultiplexer.Connect(redisSettings.ConnectionString);
});

builder.Services
    .AddInfrastructure()
    .AddApplication();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
