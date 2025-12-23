using CandidateProfiler.Application.Constants;
using CandidateProfiler.Application.Processors;
using CandidateProfiler.Application.Processors.Abstractions;
using CandidateProfiler.Application.Services;
using CandidateProfiler.Application.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var appConfigLoader = new AppConfigLoader();
var appConfig = appConfigLoader.LoadConfig(ConfigurationPaths.AppConfig);

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton(appConfig);
        services.AddSingleton<IAppConfigLoader, AppConfigLoader>();
        services.AddSingleton<IDocumentReader, PdfReader>();
        services.AddSingleton<IPromptLoader, PromptLoader>();
        services.AddSingleton<ITaskConfigLoader, TaskConfigLoader>();
        services.AddSingleton<ITempStorageService, TempStorageService>();
        services.AddSingleton<IPromptService, PromptService>();
        services.AddSingleton<ITemplateService, TemplateService>();
        services.AddSingleton<ITemplateConfigLoader, TemplateConfigLoader>();
        services.AddSingleton<ILlmConfigLoader, LlmConfigLoader>();
        services.AddSingleton<IReportBuilder, ReportBuilder>();
        services.AddHttpClient<ILlmService, OllamaLlmService>();
        services.AddSingleton<IResumesProcessor, ResumesProcessor>();
    });

var app = builder.Build();
var processor = app.Services.GetRequiredService<IResumesProcessor>();

await processor.RunAsync(appConfig.ConfigFiles.Task, appConfig.Paths.Input);