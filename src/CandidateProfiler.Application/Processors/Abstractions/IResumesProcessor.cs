namespace CandidateProfiler.Application.Processors.Abstractions;

public interface IResumesProcessor
{
    Task RunAsync(string inputDirectory, string promptTemplatePath);
}