using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Services.Abstractions;
using Newtonsoft.Json;

namespace CandidateProfiler.Application.Services;

public class TemplateConfigLoader : ITemplateConfigLoader
{
    public TemplateConfig LoadConfig(string path)
    {
        var jsonContent = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<TemplateConfig>(jsonContent) 
               ?? throw new InvalidOperationException($"Failed to load template config from {path}");
    }
}
