using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Services.Abstractions;
using Newtonsoft.Json;

namespace CandidateProfiler.Application.Services;

public class AppConfigLoader : IAppConfigLoader
{
    public AppConfig LoadConfig(string path)
    {
        var jsonContent = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<AppConfig>(jsonContent) 
               ?? throw new InvalidOperationException($"Failed to load application config from {path}");
    }
}
