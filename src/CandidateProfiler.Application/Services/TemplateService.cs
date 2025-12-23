using CandidateProfiler.Application.Services.Abstractions;

namespace CandidateProfiler.Application.Services;

public class TemplateService : ITemplateService
{
    public string LoadTemplate(string path) => File.ReadAllText(path);

    public string ReplaceTokens(string template, IDictionary<string, string> replacements)
    {
        foreach (var kvp in replacements)
            template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
        return template;
    }
}
