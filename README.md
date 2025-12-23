<p align="center">
  <a href="https://github.com/maurogioberti" target="_blank">
    <img alt="Mauro Gioberti" src="https://www.maurogioberti.com/assets/profile/maurogioberti-avatar.png" width="200" />
  </a>
</p>

<h1 align="center">
  llm-candidate-profiler 🧠
</h1>

<p align="center">
  A production-ready C# pipeline to analyze and rank resumes using Large Language Models.
  <br />
  Extracts candidate profiles, generates structured scoring, and produces beautiful HTML reports.
  <br /><br />
  <a href="https://github.com/maurogioberti/llm-candidate-profiler/stargazers">⭐ Leave a star if you like it!</a>
  &nbsp;•&nbsp;
  <a href="https://github.com/maurogioberti/llm-candidate-profiler/issues">💬 Found an issue? Report it here!</a>
</p>

<p align="center">
  <a href="https://github.com/maurogioberti/llm-candidate-profiler" title="C# LLM Resume Profiler" target="_blank">
    <img src="https://img.shields.io/badge/Built_with-.NET_8-blue?style=for-the-badge" alt="Built with .NET 8" />
  </a>
  <a href="https://github.com/maurogioberti/llm-candidate-profiler" title="Test Coverage" target="_blank">
    <img src="https://img.shields.io/badge/Tests-64_Passing-success?style=for-the-badge" alt="64 Tests Passing" />
  </a>
</p>

---

## 🚀 What's This About?

**llm-candidate-profiler** is a **production-ready** C# pipeline for automated resume analysis and candidate ranking.

### Key Features

✅ **Multi-LLM Support** – Switch between Ollama and OpenAI via configuration  
✅ **PDF Processing** – Extracts text from resume PDFs automatically  
✅ **Structured Analysis** – Scores candidates across multiple dimensions  
✅ **HTML Reports** – Generates beautiful, interactive candidate comparison reports  
✅ **Progress Tracking** – Resumes processing from interruptions  
✅ **Fully Tested** – 64 unit tests covering core functionality  
✅ **Factory Pattern** – Clean, extensible LLM provider architecture  

---

## 🧭 Processing Flow

1. **Load Configuration** – Reads LLM provider settings from `app_config.json`
2. **Scan PDFs** – Finds all resume PDFs in the input directory
3. **Extract Text** – Uses PDF reader to extract full text content
4. **Build Prompts** – Constructs analysis prompts from templates
5. **LLM Analysis** – Sends prompts to configured LLM (Ollama/OpenAI)
6. **Parse Results** – Extracts structured JSON from LLM responses
7. **Generate Report** – Creates comprehensive HTML report with charts and rankings
8. **Track Progress** – Saves intermediate results to resume on failure

---

## 📂 Project Structure

```
/llm-candidate-profiler
/llm-candidate-profiler
├── src
│   ├── CandidateProfiler.ConsoleApp
│   │   └── Program.cs                   # DI configuration & startup
│   └── CandidateProfiler.Application
│       ├── Constants
│       │   ├── ApplicationConstants.cs
│       │   └── ConfigurationPaths.cs
│       ├── Domain
│       │   ├── Config                   # Configuration models
│       │   └── Models                   # Domain models
│       ├── Helpers
│       │   └── JsonHelper.cs
│       ├── Processors
│       │   └── ResumesProcessor.cs      # Main pipeline
│       └── Services
│           ├── Abstractions              # Interfaces (SOLID)
│           ├── LlmServiceFactory.cs      # Factory pattern
│           ├── OllamaLlmService.cs       # Ollama integration
│           ├── OpenAiLlmService.cs       # OpenAI integration
│           ├── ReportBuilder.cs          # HTML reports
│           └── ... (11 service classes)
├── tests
│   └── CandidateProfiler.Application.Tests  # 64 tests
│       ├── Helpers
│       └── Services                     # 11 test files
└── Data
    ├── Config                            # Configuration files
    ├── Templates                         # HTML templates
    ├── Input                             # PDF resumes
    ├── Output                            # Results
    └── Tmp                               # Progress snapshots
```


---

## ⚡ Configuration

### Switching LLM Providers

Edit `Data/Config/app_config.json`:

**For Ollama (local):**
```json
{
  "LlmProvider": "Ollama",
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "ModelName": "llama2",
    "Temperature": 0,
    "TimeoutMinutes": 25
  }
}
```

**For OpenAI (cloud):**
```json
{
  "LlmProvider": "OpenAi",
  "OpenAi": {
    "BaseUrl": "https://api.openai.com/v1",
    "ModelName": "gpt-3.5-turbo",
    "ApiKey": "sk-your-api-key-here",
    "Temperature": 0,
    "TimeoutMinutes": 25,
    "DelayMilliseconds": 3000
  }
}
```

> 💡 **Tip**: Provider names are case-insensitive

---

## 🛠️ Getting Started

### Prerequisites

- **.NET 8 SDK** installed
- **Ollama** running locally (if using Ollama) OR **OpenAI API key** (if using OpenAI)

### Quick Start

1. **Clone the repository:**
   ```bash
   git clone https://github.com/maurogioberti/llm-candidate-profiler.git
   cd llm-candidate-profiler
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Configure your LLM provider:**
   - Edit `Data/Config/app_config.json`
   - Set `LlmProvider` to either `"Ollama"` or `"OpenAi"`

4. **Add resume PDFs:**
   ```bash
   # Place your PDF resumes in the input folder
   cp /path/to/resumes/*.pdf Data/Input/
   ```

5. **Run the application:**
   ```bash
   dotnet run --project src/CandidateProfiler.ConsoleApp
   ```

6. **View results:**
   - JSON files in `Data/Output/`
   - HTML report in `Data/Output/report.html`

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test file
dotnet test --filter "FullyQualifiedName~LlmServiceFactoryTests"
```

**Current Test Status:** ✅ 64/64 tests passing

---

## 📓 License

This project is released under the MIT License.  
You are free to use it, modify it, and share it.  
Just remember to give proper credit to the original author.