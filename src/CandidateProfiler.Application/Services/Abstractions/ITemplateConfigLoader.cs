using CandidateProfiler.Application.Domain.Config;

namespace CandidateProfiler.Application.Services.Abstractions;

public interface ITemplateConfigLoader
{
    TemplateConfig LoadConfig(string path);
}
