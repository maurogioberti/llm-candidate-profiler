using System.Diagnostics;
using System.Text.Json;
using CandidateProfiler.Application.Constants;
using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Domain.Models;
using CandidateProfiler.Application.Processors.Abstractions;
using CandidateProfiler.Application.Services.Abstractions;

namespace CandidateProfiler.Application.Processors;

public class ResumesProcessor : IResumesProcessor
{
    private readonly IDocumentReader _reader;
    private readonly ILlmService _llm;
    private readonly ITaskConfigLoader _configLoader;
    private readonly IReportBuilder _reportBuilder;
    private readonly IPromptService _promptService;
    private readonly AppConfig _appConfig;

    public ResumesProcessor(
        IDocumentReader reader,
        ILlmService llm,
        ITaskConfigLoader configLoader,
        IReportBuilder reportBuilder,
        IPromptService promptService,
        AppConfig appConfig)
    {
        _reader = reader;
        _llm = llm;
        _configLoader = configLoader;
        _reportBuilder = reportBuilder;
        _promptService = promptService;
        _appConfig = appConfig;
    }

    public async Task RunAsync(string inputConfigurations, string inputDirectory)
    {
        var inputFiles = Directory.GetFiles(inputDirectory, FileExtensions.Pdf);
        var rawConfigFile = await File.ReadAllTextAsync(inputConfigurations);
        var configClass = JsonSerializer.Deserialize<RagTaskConfig>(rawConfigFile)!;
        var assets = _configLoader.LoadAssetsAsync(configClass);

        if (!inputFiles.Any())
        {
            Console.WriteLine(ConsoleMessages.NoPdfFilesFound);
            return;
        }

        await ProcessInputFiles(inputFiles, assets);

        var jsonFiles = Directory.GetFiles(_appConfig.Paths.Output, FileExtensions.Json);

        if (!jsonFiles.Any())
        {
            Console.WriteLine(ConsoleMessages.NoJsonFilesFound);
            return;
        }

        var report = await _reportBuilder.GenerateHtmlReportAsync(jsonFiles);

        var reportFile = Path.Combine(_appConfig.Paths.Output, FileNames.Report);
        await File.WriteAllTextAsync(reportFile, report);
        Console.WriteLine(string.Format(ConsoleMessages.ReportGenerated, reportFile));

        Process.Start(new ProcessStartInfo
        {
            FileName = reportFile,
            UseShellExecute = true
        });

        Console.WriteLine(ConsoleMessages.AllProcessed);
    }

    private async Task ProcessInputFiles(string[] inputFiles, LoadedTaskAssets assets)
    {
        foreach (var file in inputFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var outputFile = Path.Combine(_appConfig.Paths.Output, $"{fileName}{FileExtensions.Json.TrimStart('*')}");

            if (File.Exists(outputFile))
            {
                Console.WriteLine(string.Format(ConsoleMessages.FileAlreadyExists, fileName));
                continue;
            }

            Console.WriteLine(string.Format(ConsoleMessages.ProcessingFile, fileName));

            var pages = await _reader.ReadPagesAsync(file);
            var fullText = string.Join(Environment.NewLine, pages);
            var prompt = _promptService.PreparePrompt(assets.PromptTemplate, fullText);
            var jsonResult = await _llm.CompleteAsync(prompt);

            Directory.CreateDirectory(_appConfig.Paths.Output);
            await File.WriteAllTextAsync(outputFile, jsonResult);

            Console.WriteLine(string.Format(ConsoleMessages.FileSaved, fileName));
        }
    }
}