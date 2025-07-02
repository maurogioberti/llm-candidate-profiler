using System.Text;
using CandidateProfiler.Application.Helpers;
using Newtonsoft.Json.Linq;

namespace CandidateProfiler.Application.Services;

public interface IReportBuilder
{
    Task<string> GenerateHtmlReportAsync(IEnumerable<string> jsonFiles);
}

public class ReportBuilder : IReportBuilder
{
    public async Task<string> GenerateHtmlReportAsync(IEnumerable<string> jsonFiles)
    {
        var reportBuilder = new StringBuilder();
        reportBuilder.AppendLine(@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1'>
            <title>Comprehensive Candidate Analysis</title>
            <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css' rel='stylesheet'>
            <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
        </head>
        <body>
        <div class='container py-5'>
            <h1 class='mb-4'>Comprehensive Candidate Analysis</h1>
            <div class='row row-cols-1 row-cols-md-2 g-4'>");

        var candidateIndex = 0;

        foreach (var jsonFile in jsonFiles)
        {
            var jsonContent = await File.ReadAllTextAsync(jsonFile);
            var jsonObject = JsonHelper.TryParseJson(jsonContent);

            if (jsonObject is null)
            {
                Console.WriteLine($"Archivo JSON inválido: {jsonFile}, skipping...");
                continue;
            }

            var generalInfo = jsonObject["GeneralInfo"];
            var scoresInfo = jsonObject["Scores"];
            var relevanceInfo = jsonObject["RelevanceToTargetRole"];
            var keywordCoverage = jsonObject["KeywordCoverage"];

            var fullName = generalInfo?["Fullname"]?.ToString() ?? "N/A";
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

            // HTML para cada candidato
            reportBuilder.AppendLine($@"
    <div class='col'>
        <div class='card h-100 shadow-sm'>
            <div class='card-body'>
                <h5 class='card-title'>{fullName} {titleDetected}</h5>
                <p class='card-text'><strong>Seniority:</strong> {seniority}</p>
                <p class='card-text'><strong>Industry:</strong> {industry}</p>
                <p class='card-text'><strong>English Level:</strong> {english}</p>
                <p class='card-text'><strong>General Score:</strong> {generalScore}</p>
                <canvas id='skillsCoverageChart{candidateIndex}'></canvas>
                <canvas id='relevancePieChart{candidateIndex}' class='mt-4'></canvas>
            </div>
        </div>
    </div>");

            candidateIndex++;
        }

        reportBuilder.AppendLine(@"
    </div>
</div>
<script>");

        candidateIndex = 0;

        foreach (var jsonFile in jsonFiles)
        {
            var jsonContent = await File.ReadAllTextAsync(jsonFile);
            var jsonObject = JsonHelper.TryParseJson(jsonContent);

            if (jsonObject is null) continue;

            var keywordCoverage = jsonObject["KeywordCoverage"];
            var relevanceInfo = jsonObject["RelevanceToTargetRole"];

            var keywordsDetectedCount = keywordCoverage?["KeywordsDetected"]?.Count() ?? 0;
            var keywordsMissingCount = keywordCoverage?["KeywordsMissing"]?.Count() ?? 0;

            var titleMatch = MapFit(relevanceInfo?["TitleMatch"]?.ToString() ?? "");
            var responsibilityMatch = MapFit(relevanceInfo?["ResponsibilityMatch"]?.ToString() ?? "");
            var overallFit = MapFit(relevanceInfo?["OverallFit"]?.ToString() ?? "");

            reportBuilder.AppendLine($@"
    new Chart(document.getElementById('skillsCoverageChart{candidateIndex}'), {{
        type: 'doughnut',
        data: {{
            labels: ['Detected', 'Missing'],
            datasets: [{{
                data: [{keywordsDetectedCount}, {keywordsMissingCount}],
                backgroundColor: ['#36A2EB', '#FF6384']
            }}]
        }}
    }});

    new Chart(document.getElementById('relevancePieChart{candidateIndex}'), {{
        type: 'pie',
        data: {{
            labels: ['Title Match', 'Responsibility Match', 'Overall Fit'],
            datasets: [{{
                data: [{titleMatch}, {responsibilityMatch}, {overallFit}],
                backgroundColor: ['#FFCE56', '#4BC0C0', '#9966FF']
            }}]
        }}
    }});");

            candidateIndex++;
        }

        reportBuilder.AppendLine(@"
</script>
</body>
</html>");

        return reportBuilder.ToString();
    }

    private int MapFit(string fit)
    {
        return fit switch
        {
            "Low" => 25,
            "Moderate" => 50,
            "Fair" => 60,
            "High" => 75,
            "Excellent" => 100,
            _ => 0
        };
    }
}
