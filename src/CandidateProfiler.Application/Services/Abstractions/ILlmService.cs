namespace CandidateProfiler.Application.Services.Abstractions;

public interface ILlmService
{
    Task<string> CompleteAsync(string prompt);
}