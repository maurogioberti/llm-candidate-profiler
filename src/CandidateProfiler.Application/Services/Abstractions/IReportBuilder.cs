namespace CandidateProfiler.Application.Services.Abstractions;

public interface IReportBuilder
{
    Task<string> GenerateHtmlReportAsync(IEnumerable<string> jsonFiles);
}
