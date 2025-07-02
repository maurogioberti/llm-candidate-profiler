namespace CandidateProfiler.Application.Services.Abstractions;

public interface IDocumentReader
{
    Task<IReadOnlyList<string>> ReadPagesAsync(string filePath);
}