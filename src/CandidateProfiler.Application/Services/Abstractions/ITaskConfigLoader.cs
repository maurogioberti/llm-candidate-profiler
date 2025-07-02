using CandidateProfiler.Application.Domain.Models;

namespace CandidateProfiler.Application.Services.Abstractions;

public interface ITaskConfigLoader
{
    LoadedTaskAssets LoadAssetsAsync(RagTaskConfig config);
}