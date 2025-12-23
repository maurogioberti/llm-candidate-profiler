using CandidateProfiler.Application.Domain.Config;

namespace CandidateProfiler.Application.Services.Abstractions;

public interface IAppConfigLoader
{
    AppConfig LoadConfig(string path);
}
