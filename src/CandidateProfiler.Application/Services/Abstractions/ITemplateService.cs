namespace CandidateProfiler.Application.Services.Abstractions;

public interface ITemplateService
{
    string LoadTemplate(string path);
    string ReplaceTokens(string template, IDictionary<string, string> replacements);
}
