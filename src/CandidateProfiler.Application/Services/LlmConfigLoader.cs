using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Services.Abstractions;
using Newtonsoft.Json;

namespace CandidateProfiler.Application.Services;

public class LlmConfigLoader : ILlmConfigLoader
{
    public LlmConfig LoadConfig(string path)
    {
        var jsonContent = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<LlmConfig>(jsonContent) 
               ?? throw new InvalidOperationException($"Failed to load LLM config from {path}");
    }
}
