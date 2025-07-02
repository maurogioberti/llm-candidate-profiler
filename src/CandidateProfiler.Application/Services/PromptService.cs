using CandidateProfiler.Application.Services.Abstractions;

namespace CandidateProfiler.Application.Services;

public class PromptService : IPromptService
{
    public string PreparePrompt(string template, string processedText) => template.Replace("_@{PROCESSED_TEXT}", processedText);
}