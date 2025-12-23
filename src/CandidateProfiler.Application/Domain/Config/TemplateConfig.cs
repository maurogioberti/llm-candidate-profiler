namespace CandidateProfiler.Application.Domain.Config;

public class TemplateConfig
{
    public BadgeMappings BadgeMappings { get; set; } = new();
    public Dictionary<string, int> FitMappings { get; set; } = new();
}

public class BadgeMappings
{
    public Dictionary<string, string> Seniority { get; set; } = new();
    public Dictionary<string, string> Industry { get; set; } = new();
    public Dictionary<string, string> EnglishLevel { get; set; } = new();
}
