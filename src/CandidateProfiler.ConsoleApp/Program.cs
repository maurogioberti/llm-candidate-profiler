using CandidateProfiler.Application.Processors;
using CandidateProfiler.Application.Processors.Abstractions;
using CandidateProfiler.Application.Services;
using CandidateProfiler.Application.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IDocumentReader, PdfReader>();
        services.AddSingleton<IPromptLoader, PromptLoader>();
        services.AddSingleton<ITaskConfigLoader, TaskConfigLoader>();
        services.AddSingleton<ITempStorageService, TempStorageService>();
        services.AddSingleton<IPromptService, PromptService>();
        services.AddHttpClient<ILlmService, DeepSeekLlmService>();
        services.AddSingleton<IResumesProcessor, ResumesProcessor>();
    });

var app = builder.Build();
var processor = app.Services.GetRequiredService<IResumesProcessor>();

await processor.RunAsync("Data/Config/task_config.json", "Data/Input");