using System.Text;
using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Helpers;
using CandidateProfiler.Application.Services.Abstractions;
using Newtonsoft.Json.Linq;

namespace CandidateProfiler.Application.Services;

public class ReportBuilder : IReportBuilder
{
    private readonly ITemplateService _templateService;
    private readonly ITemplateConfigLoader _templateConfigLoader;
    private readonly TemplateConfig _templateConfig;
    private readonly AppConfig _appConfig;

    public ReportBuilder(ITemplateService templateService, ITemplateConfigLoader templateConfigLoader, AppConfig appConfig)
    {
        _templateService = templateService;
        _templateConfigLoader = templateConfigLoader;
        _appConfig = appConfig;
        _templateConfig = _templateConfigLoader.LoadConfig(appConfig.ConfigFiles.TemplateConfig);
    }

    public async Task<string> GenerateHtmlReportAsync(IEnumerable<string> jsonFiles)
    {
        var candidateCards = new StringBuilder();
        var chartScripts = new StringBuilder();
        int candidateIndex = 0;

        var candidatesData = new List<(string FullName, int GeneralScore)>();

        foreach (var jsonFile in jsonFiles)
        {
            var jsonContent = await File.ReadAllTextAsync(jsonFile);
            var jsonObject = JsonHelper.TryParseJson(jsonContent);
            if (jsonObject is null) continue;

            var generalInfo = jsonObject["GeneralInfo"];
            var scoresInfo = jsonObject["Scores"];
            var fullName = generalInfo?["Fullname"]?.ToString() ?? "N/A";
            var generalScore = scoresInfo?["GeneralScore"]?.Value<int>() ?? 0;
            candidatesData.Add((fullName, generalScore));
        }

        var bestCandidate = candidatesData.MaxBy(c => c.GeneralScore);
        var bestScore = bestCandidate.GeneralScore;
        var bestCandidateName = bestCandidate.FullName;
        var avgScore = candidatesData.Any() ? candidatesData.Average(x => x.GeneralScore) : 0;

        foreach (var jsonFile in jsonFiles)
        {
            var jsonContent = await File.ReadAllTextAsync(jsonFile);
            var jsonObject = JsonHelper.TryParseJson(jsonContent);

            if (jsonObject is null)
            {
                Console.WriteLine($"Invalid JSON file: {jsonFile}, skipping...");
                continue;
            }

            BuildCandidateCard(jsonObject, candidateIndex, candidateCards, chartScripts);
            candidateIndex++;
        }

        var template = _templateService.LoadTemplate(_appConfig.Templates.Report);
        var replacements = new Dictionary<string, string>
        {
            { "TOTAL_CANDIDATES", candidatesData.Count.ToString() },
            { "TOP_SCORE", bestScore.ToString() },
            { "TOP_CANDIDATE", bestCandidateName },
            { "AVG_SCORE", $"{avgScore:0.0}" },
            { "CANDIDATE_CARDS", candidateCards.ToString() },
            { "CHART_SCRIPTS", chartScripts.ToString() }
        };

        return _templateService.ReplaceTokens(template, replacements);
    }

    private void BuildCandidateCard(JObject jsonObject, int candidateIndex, StringBuilder candidateCards, StringBuilder chartScripts)
    {
        var generalInfo = jsonObject["GeneralInfo"];
        var scoresInfo = jsonObject["Scores"];
        var relevanceInfo = jsonObject["RelevanceToTargetRole"];
        var keywordCoverage = jsonObject["KeywordCoverage"];

        var fullName = generalInfo?["Fullname"]?.ToString() ?? "N/A";
        var initials = GetInitials(fullName);
        var titleDetected = generalInfo?["TitleDetected"]?.ToString() ?? "N/A";
        var seniority = generalInfo?["SeniorityLevel"]?.ToString() ?? "N/A";
        var industry = generalInfo?["MainIndustry"]?.ToString() ?? "N/A";
        var english = generalInfo?["EnglishLevel"]?.ToString() ?? "N/A";
        var generalScore = scoresInfo?["GeneralScore"]?.Value<int>() ?? 0;
        var keywordsDetectedCount = keywordCoverage?["KeywordsDetected"]?.Count() ?? 0;
        var keywordsMissingCount = keywordCoverage?["KeywordsMissing"]?.Count() ?? 0;

        var titleMatch = MapFit(relevanceInfo?["TitleMatch"]?.ToString() ?? "");
        var responsibilityMatch = MapFit(relevanceInfo?["ResponsibilityMatch"]?.ToString() ?? "");
        var overallFit = MapFit(relevanceInfo?["OverallFit"]?.ToString() ?? "");

        var cardTemplate = _templateService.LoadTemplate(_appConfig.Templates.CandidateCard);
        var cardReplacements = new Dictionary<string, string>
        {
            { "INITIALS", initials },
            { "FULL_NAME", fullName },
            { "TITLE_DETECTED", titleDetected },
            { "SENIORITY", seniority },
            { "SENIORITY_BADGE", GetSeniorityBadge(seniority) },
            { "INDUSTRY", industry },
            { "INDUSTRY_BADGE", GetIndustryBadge(industry) },
            { "ENGLISH", english },
            { "ENGLISH_BADGE", GetEnglishBadge(english) },
            { "GENERAL_SCORE", generalScore.ToString() },
            { "KEYWORDS_DETECTED", keywordsDetectedCount.ToString() },
            { "KEYWORDS_MISSING", keywordsMissingCount.ToString() },
            { "OVERALL_FIT", overallFit.ToString() },
            { "CANDIDATE_INDEX", candidateIndex.ToString() }
        };
        candidateCards.AppendLine(_templateService.ReplaceTokens(cardTemplate, cardReplacements));

        var chartTemplate = _templateService.LoadTemplate(_appConfig.Templates.ChartScript);
        var chartReplacements = new Dictionary<string, string>
        {
            { "CANDIDATE_INDEX", candidateIndex.ToString() },
            { "KEYWORDS_DETECTED", keywordsDetectedCount.ToString() },
            { "KEYWORDS_MISSING", keywordsMissingCount.ToString() },
            { "TITLE_MATCH", titleMatch.ToString() },
            { "RESPONSIBILITY_MATCH", responsibilityMatch.ToString() },
            { "OVERALL_FIT", overallFit.ToString() }
        };
        chartScripts.AppendLine(_templateService.ReplaceTokens(chartTemplate, chartReplacements));
    }

    private string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "N/A";
        var parts = name.Trim().Split(' ');
        if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
        return string.Concat(parts[0][0], parts[^1][0]).ToUpper();
    }

    private string GetSeniorityBadge(string seniority)
    {
        var key = seniority.ToLower();
        return _templateConfig.BadgeMappings.Seniority.TryGetValue(key, out var badge)
            ? badge
            : _templateConfig.BadgeMappings.Seniority.GetValueOrDefault("default", "bg-secondary");
    }

    private string GetIndustryBadge(string industry)
    {
        return string.IsNullOrWhiteSpace(industry)
            ? _templateConfig.BadgeMappings.Industry.GetValueOrDefault("empty", "bg-secondary")
            : _templateConfig.BadgeMappings.Industry.GetValueOrDefault("default", "bg-dark");
    }

    private string GetEnglishBadge(string level)
    {
        var key = level.ToLower();
        return _templateConfig.BadgeMappings.EnglishLevel.TryGetValue(key, out var badge)
            ? badge
            : _templateConfig.BadgeMappings.EnglishLevel.GetValueOrDefault("default", "bg-secondary");
    }

    private int MapFit(string fit)
    {
        var key = fit.ToLower();
        return _templateConfig.FitMappings.TryGetValue(key, out var value)
            ? value
            : _templateConfig.FitMappings.GetValueOrDefault("default", 0);
    }
}
