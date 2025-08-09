using System.Text;
using CandidateProfiler.Application.Helpers;
using CandidateProfiler.Application.Services.Abstractions;
using Newtonsoft.Json.Linq;

namespace CandidateProfiler.Application.Services;
//TODO: This is a very basic report builder. It can be improved with more advanced features like filtering, sorting, and better UI/UX.
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
    <title>Candidate Profiler Report</title>
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css' rel='stylesheet'>
    <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
    <style>
        body { background: #f6f7fb; }
        .card { border-radius: 20px; }
        .badge-custom { font-size: 1em; margin-right: 5px;}
        .candidate-avatar {
            width: 48px;
            height: 48px;
            background: #495057;
            color: #fff;
            border-radius: 50%;
            font-size: 2em;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-right: 1rem;
        }
        .score-bar { height: 16px; border-radius: 8px; }
        .progress-fit { height: 14px; }
    </style>
</head>
<body>
<div class='container py-5'>
    <div class='d-flex justify-content-between align-items-center mb-4'>
        <h1 class='fw-bold'>Candidate Profiler Report</h1>
        <a href='#' onclick='window.print(); return false;' class='btn btn-outline-secondary'>Download as PDF</a>
    </div>
    <div class='mb-5'>
        <div class='alert alert-info shadow-sm'>
            <strong>Tip:</strong> Hover over each section for more info. <span class='ms-2'>??</span>
        </div>
    </div>
    <div class='row row-cols-1 row-cols-md-2 g-4'>
");

        int totalCandidates = 0;
        int bestScore = 0;
        string bestCandidate = "";

        var candidateCards = new List<string>();
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

            if (generalScore > bestScore)
            {
                bestScore = generalScore;
                bestCandidate = fullName;
            }
        }

        reportBuilder.AppendLine($@"
        <div class='row mb-4'>
            <div class='col-md-4'>
                <div class='card p-3 text-center shadow-sm'>
                    <div class='fs-1 fw-bold text-primary'>{candidatesData.Count}</div>
                    <div class='fw-light'>Candidates Analyzed</div>
                </div>
            </div>
            <div class='col-md-4'>
                <div class='card p-3 text-center shadow-sm'>
                    <div class='fs-1 fw-bold text-success'>{bestScore}</div>
                    <div class='fw-light'>Top General Score</div>
                    <small class='text-muted'>{bestCandidate}</small>
                </div>
            </div>
            <div class='col-md-4'>
                <div class='card p-3 text-center shadow-sm'>
                    <div class='fs-1 fw-bold text-info'>{candidatesData.Average(x => x.GeneralScore):0.0}</div>
                    <div class='fw-light'>Average General Score</div>
                </div>
            </div>
        </div>
        <hr />
        ");

        candidateIndex = 0;
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

            string seniorityBadge = GetSeniorityBadge(seniority);
            string industryBadge = GetIndustryBadge(industry);
            string englishBadge = GetEnglishBadge(english);

            candidateCards.Add($@"
    <div class='col'>
        <div class='card h-100 shadow-sm'>
            <div class='card-body d-flex flex-column'>
                <div class='d-flex align-items-center mb-3'>
                    <div class='candidate-avatar me-3' title='Candidate initials'>{initials}</div>
                    <div>
                        <h5 class='card-title mb-0'>{fullName}</h5>
                        <div class='fw-light text-muted' title='Detected Title'>{titleDetected}</div>
                    </div>
                </div>
                <div class='mb-2'>
                    <span class='badge badge-custom {seniorityBadge}' title='Seniority'>{seniority}</span>
                    <span class='badge badge-custom {industryBadge}' title='Main Industry'>{industry}</span>
                    <span class='badge badge-custom {englishBadge}' title='English Level'>{english}</span>
                </div>
                <div class='mb-3'>
                    <strong>General Score:</strong>
                    <span class='badge bg-primary fs-6'>{generalScore}</span>
                    <div class='progress score-bar mt-1' title='General Score'>
                        <div class='progress-bar bg-primary' style='width:{generalScore}%'>{generalScore}%</div>
                    </div>
                </div>
                <div>
                    <strong>Skills Coverage:</strong>
                    <span title='Keywords detected'><span class='text-success fw-bold'>{keywordsDetectedCount}</span> found</span>
                    <span class='mx-2'>/</span>
                    <span title='Keywords missing'><span class='text-danger fw-bold'>{keywordsMissingCount}</span> missing</span>
                </div>
                <div class='mt-3'>
                    <canvas id='skillsCoverageChart{candidateIndex}' height='80'></canvas>
                </div>
                <div class='mt-4'>
                    <strong>Role Fit:</strong>
                    <div class='progress progress-fit mt-1 mb-2'>
                        <div class='progress-bar bg-success' style='width:{overallFit}%'>{overallFit}%</div>
                    </div>
                    <canvas id='relevancePieChart{candidateIndex}' height='80'></canvas>
                </div>
                <div class='mt-3 d-flex justify-content-between'>
                    <a href='#feedback' class='btn btn-sm btn-outline-info'>Give Feedback</a>
                </div>
            </div>
        </div>
    </div>
");

            chartScripts.AppendLine($@"
    new Chart(document.getElementById('skillsCoverageChart{candidateIndex}'), {{
        type: 'doughnut',
        data: {{
            labels: ['Detected', 'Missing'],
            datasets: [{{
                data: [{keywordsDetectedCount}, {keywordsMissingCount}],
                backgroundColor: ['#36A2EB', '#FF6384']
            }}]
        }},
        options: {{
            plugins: {{
                legend: {{ display: false }},
                tooltip: {{
                    callbacks: {{
                        label: function(context) {{
                            return context.label + ': ' + context.raw;
                        }}
                    }}
                }}
            }}
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
        }},
        options: {{
            plugins: {{
                legend: {{ position: 'bottom' }},
                tooltip: {{
                    callbacks: {{
                        label: function(context) {{
                            return context.label + ': ' + context.raw + '%';
                        }}
                    }}
                }}
            }}
        }}
    }});
");
            candidateIndex++;
        }

        reportBuilder.AppendLine(string.Join(Environment.NewLine, candidateCards));

        reportBuilder.AppendLine(@"
        </div>
        <div class='text-center my-5' id='feedback'>
            <hr />
            <h4>?? HR Feedback</h4>
            <p>If you have suggestions or need more info, contact <a href='mailto:hr-team@yourcompany.com'>hr-team@yourcompany.com</a></p>
        </div>
    </div>
    <script>
        " + chartScripts.ToString() + @"
    </script>
</body>
</html>
");

        return reportBuilder.ToString();
    }

    private string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "N/A";
        var parts = name.Trim().Split(' ');
        if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
        return string.Concat(parts[0][0], parts[^1][0]).ToUpper();
    }
    private string GetSeniorityBadge(string seniority)
        => seniority.ToLower() switch
        {
            "junior" => "bg-info",
            "mid" => "bg-warning text-dark",
            "senior" => "bg-success",
            "lead" => "bg-danger",
            _ => "bg-secondary"
        };
    private string GetIndustryBadge(string industry)
        => string.IsNullOrWhiteSpace(industry) ? "bg-secondary" : "bg-dark";
    private string GetEnglishBadge(string level)
        => level.ToLower() switch
        {
            "beginner" => "bg-danger",
            "intermediate" => "bg-warning text-dark",
            "advanced" => "bg-primary",
            "native" => "bg-success",
            _ => "bg-secondary"
        };
    private int MapFit(string fit)
        => fit.ToLower() switch
        {
            "low" => 25,
            "moderate" => 50,
            "fair" => 60,
            "high" => 75,
            "excellent" => 100,
            _ => 0
        };
}
