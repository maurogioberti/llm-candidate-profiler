namespace CandidateProfiler.Application.Services.Abstractions;

public interface IPromptService
{
    string PreparePrompt(string template, string processedText);
}