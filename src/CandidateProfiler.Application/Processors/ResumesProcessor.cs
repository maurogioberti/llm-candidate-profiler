using System.Diagnostics;
using System.Text.Json;
using CandidateProfiler.Application.Domain.Models;
using CandidateProfiler.Application.Processors.Abstractions;
using CandidateProfiler.Application.Services.Abstractions;

namespace CandidateProfiler.Application.Processors;

public class ResumesProcessor(
        IDocumentReader reader,
        ILlmService llm,
        ITaskConfigLoader configLoader,
        IReportBuilder reportBuilder,
        IPromptService promptService) : IResumesProcessor
{

    public async Task RunAsync(string inputConfigurations, string inputDirectory)
    {
        var inputFiles = Directory.GetFiles(inputDirectory, "*.pdf");
        var rawConfigFile = await File.ReadAllTextAsync(inputConfigurations);
        var configClass = JsonSerializer.Deserialize<RagTaskConfig>(rawConfigFile)!;
        var assets = configLoader.LoadAssetsAsync(configClass);

        if (!inputFiles.Any())
        {
            Console.WriteLine("No PDF files found in the input directory.");
            return;
        }

        await ProcessInputFile(reader, llm, promptService, inputFiles, assets);

        var outputDir = Path.Combine("Data", "Output");

        var jsonFiles = Directory.GetFiles(outputDir, "*.json");

        if (!jsonFiles.Any())
        {
            Console.WriteLine("No JSON files found in the output directory.");
            return;
        }

        var report = await reportBuilder.GenerateHtmlReportAsync(jsonFiles);

        var reportFile = Path.Combine("Data", "Output", "report.html");
        await File.WriteAllTextAsync(reportFile, report);
        Console.WriteLine($"HTML report generated at: {reportFile}");

        Process.Start(new ProcessStartInfo
        {
            FileName = reportFile,
            UseShellExecute = true
        });

        Console.WriteLine("All resumes processed successfully!");
    }

    private static async Task ProcessInputFile(IDocumentReader reader, ILlmService llm, IPromptService promptService, string[] inputFiles, LoadedTaskAssets assets)
    {
        foreach (var file in inputFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var outputFile = Path.Combine("Data", "Output", $"{fileName}.json");

            if (File.Exists(outputFile))
            {
                Console.WriteLine($"Output for {fileName} already exists, skipping.");
                continue;
            }

            Console.WriteLine($"Processing {fileName}...");

            var pages = await reader.ReadPagesAsync(file);

            var fullText = string.Join(Environment.NewLine, pages);

            var prompt = promptService.PreparePrompt(assets.PromptTemplate, fullText);

            var jsonResult = await llm.CompleteAsync(prompt);

            Directory.CreateDirectory(Path.Combine("Data", "Output"));
            await File.WriteAllTextAsync(outputFile, jsonResult);

            Console.WriteLine($"Processed and saved output for {fileName}.");
        }
    }
}
