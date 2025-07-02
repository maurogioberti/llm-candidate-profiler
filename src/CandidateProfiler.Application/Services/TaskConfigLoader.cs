using CandidateProfiler.Application.Domain.Models;
using CandidateProfiler.Application.Services.Abstractions;

namespace CandidateProfiler.Application.Services;

public class TaskConfigLoader : ITaskConfigLoader
{
    private readonly IPromptLoader _promptLoader;

    public TaskConfigLoader(IPromptLoader promptLoader)
    {
        _promptLoader = promptLoader;
    }

    public LoadedTaskAssets LoadAssetsAsync(RagTaskConfig config)
    {
        var promptTemplate = _promptLoader.LoadPrompt(config.PromptPath);

        return new LoadedTaskAssets(promptTemplate);
    }
}