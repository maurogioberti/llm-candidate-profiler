namespace CandidateProfiler.Application.Services.Abstractions;

public interface IPromptLoader
{
    string LoadPrompt(string path);
}