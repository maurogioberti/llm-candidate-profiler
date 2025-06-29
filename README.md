<p align="center">
  <a href="https://github.com/maurogioberti" target="_blank">
    <img alt="Mauro Gioberti" src="https://www.maurogioberti.com/assets/profile/maurogioberti-avatar.png" width="200" />
  </a>
</p>

<h1 align="center">
  llm-candidate-profiler 🧠
</h1>
<p align="center">
  A simple C# pipeline to analyze and rank resumes using Large Language Models.
  <br />
  Extracts candidate profiles and generates structured scoring.
  <br />
  <br />
  <a href="https://github.com/maurogioberti/llm-candidate-profiler/stargazers">⭐ Leave a star if you like it!</a>
  <a href="https://github.com/maurogioberti/llm-candidate-profiler/issues">💬 Found an issue? Report it here!</a>
</p>

<p align="center">
  <a href="https://github.com/maurogioberti/llm-candidate-profiler" title="C# LLM Resume Profiler" target="_blank">
    <img src="https://img.shields.io/badge/Built_with-.NET-blue?style=for-the-badge" alt="Built with .NET" />
  </a>
</p>

---

## 🚀 What’s This About?

**llm-candidate-profiler** is a **simple and focused** C# pipeline.  
Its purpose is to **process resumes**, extract key candidate information, and produce a ranking automatically via an LLM (like DeepSeek).

The idea is to keep the architecture **minimal and practical**, so it does the job without unnecessary complexity.

---

## 🧭 Planned Flow

1. **Load** PDF resumes from an input directory.
2. **Extract text** using a PDF reader service.
3. **Generate prompts** with the full text.
4. **Call the LLM** (DeepSeek or ChatGPT) for analysis.
5. **Parse JSON response** into structured candidate profiles.
6. **Store results** (CSV or JSON) for further use.

---

## 📂 Project Structure

/llm-candidate-profiler
├── src
│ ├── CandidateProfiler # Console entry point
│ │ └── Program.cs
│ │
│ └── CandidateProfiler.Application # Core logic and services
│ ├── Domain
│ │ ├── Models
│ │ │ └── CandidateProfile.cs
│ │ └── Config
│ │ └── TaskConfig.cs
│ │
│ ├── Services
│ │ ├── PdfReader.cs
│ │ ├── LlmService.cs
│ │ ├── PromptBuilder.cs
│ │ └── ResultWriter.cs
│ │
│ └── Processors
│ └── CandidatePipelineProcessor.cs
│
└── tests
└── CandidateProfiler.Tests # Unit tests


---

## ⚡ Why Keep It Simple?

This project is intentionally **not over-engineered**:

✅ Minimal layers  
✅ Easy to follow  
✅ Focused only on resume profiling and ranking

If you need more, you can extend it later.

---

## 🛠️ Getting Started

1. **Clone the repository:**
   ```bash
   git clone https://github.com/maurogioberti/llm-candidate-profiler.git
   cd llm-candidate-profiler
   ```
   
2. **Restore dependencies**:

   ```bash
   dotnet restore
   ```

3. **Set up and run the solutions**:

   ```bash
   dotnet run --project src/CandidateProfiler
   ```

---

## 📓 License

This project is released under the MIT License.  
You are free to use it, modify it, and share it.  
Just remember to give proper credit to the original author.