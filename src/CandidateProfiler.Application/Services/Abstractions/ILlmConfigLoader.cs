using CandidateProfiler.Application.Domain.Config;

namespace CandidateProfiler.Application.Services.Abstractions;

public interface ILlmConfigLoader
{
    LlmConfig LoadConfig(string path);
}
