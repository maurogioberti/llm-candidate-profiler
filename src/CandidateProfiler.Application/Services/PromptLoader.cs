using CandidateProfiler.Application.Services.Abstractions;

namespace CandidateProfiler.Application.Services;

public class PromptLoader : IPromptLoader
{
    public string LoadPrompt(string path) => File.ReadAllText(path);
    public string ReplaceTokens(string prompt, IDictionary<string, string> replacements)
    {
        foreach (var kvp in replacements)
            prompt = prompt.Replace($"_@{{{kvp.Key}}}", kvp.Value);
        return prompt;
    }
}