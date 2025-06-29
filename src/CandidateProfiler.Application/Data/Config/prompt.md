# 📄 README.md — Resume Analyzer Prompt

Toma el siguiente texto de un **Curriculum Vitae** extraído de un PDF.  
Procesa y evalúa de manera exhaustiva, generando limpieza, reescritura y un análisis completo según los criterios detallados.

---

## ✨ Instrucciones Generales

Act as an advanced Resume Analyzer and Rewriter.

You will receive the content of a resume extracted from a PDF.

Follow these instructions carefully:

---

## 🧹 1. Clean and Reformat

- Remove encoding issues, broken lines, and formatting problems.
- Reconstruct any split words or sentences.
- Keep the overall structure and sections (Experience, Education, Skills).

---

## ✍️ 2. Rewrite for Clarity

- Rewrite the text to be professional, clear, and concise.
- Do **not** change the meaning of the content.

---

## 📊 3. Extract and Analyze

Provide detailed analysis according to the sections below.

---

## ## 📋 General Information

- **TitleDetected**: the title explicitly stated by the candidate.
- **TitlePredicted**: the title you infer from the content.
- **SeniorityLevel**: Junior / Mid / Senior.
- **YearsExperience**: total years of professional experience.
- **RelevantYears**: years spent in key relevant roles.
- **TrajectoryPattern**: Stable / Variable.
- **IndustryMatch**: alignment with the target industry.
- **MainIndustry**: main industry of experience.
- **EnglishLevel**: estimated English level.
- **OtherLanguages**: list of other languages with proficiency and evidence.
- **Location**: if available.

---

## ## 🛠️ Skills Detected

For each skill:
- **SkillName**
- **SkillLevel**: Low / Medium / High / Very High.
- **Evidence**: text supporting the assessment.

---

## ## 🗝️ Keyword Coverage

- **KeywordsDetected**: list of detected keywords.
- **KeywordsMissing**: relevant keywords not found.
- **Density**: percentage of keyword density.
- **Context**: brief description of how the keywords appear.

---

## ## 🌍 Languages

For each language:
- **Language**
- **Proficiency**: Basic / Intermediate / Advanced / Native.
- **Evidence**: example text justifying the assessment.

---

## ## 📝 Clarity and Formatting

- **ClarityScore**: subjective evaluation (0–100).
- **FormattingScore**: structure and visual presentation (0–100).
- **SpellingErrors**: approximate count of spelling mistakes.

---

## ## 👔 Relevance to Target Role

- **TitleMatch**: degree of alignment with the target title.
- **ResponsibilityMatch**: degree of alignment with target responsibilities.
- **OverallFit**: High / Medium / Low.

---

## ## 🧠 Scoring

Provide a numeric score (0–100) and a short comment for each dimension:

- **GeneralScore**
- **ATSCompatibility**
- **ClarityScore**
- **FormattingScore**
- **KeywordDensity**
- **EnglishProficiency**
- **SeniorityMatch**
- **SkillCoverage**
- **AchievementsQuantification**
- **SoftSkillsCoverage**

---

## ## 🧾 Summary and Feedback

- **Summary**: one-paragraph description of the candidate profile.
- **Strengths**: top 5 strengths.
- **AreasToImprove**: top 3 areas to improve.
- **Tips**: suggestions to improve the resume.

---

## ## 🧽 Cleaned Resume Text

Provide the cleaned, improved resume text ready for export.

---

## ✨ Example Output JSON

```json
{
  "GeneralInfo": {
    "TitleDetected": "",
    "TitlePredicted": "",
    "SeniorityLevel": "",
    "YearsExperience": 0,
    "RelevantYears": 0,
    "IndustryMatch": "",
    "TrajectoryPattern": "",
    "MainIndustry": "",
    "EnglishLevel": "",
    "OtherLanguages": [],
    "Location": ""
  },
  "SkillMatrix": [],
  "KeywordCoverage": {
    "KeywordsDetected": [],
    "KeywordsMissing": [],
    "Density": 0,
    "Context": ""
  },
  "Languages": [],
  "Scores": {},
  "Relevance": {
    "TitleMatch": "",
    "ResponsibilityMatch": "",
    "OverallFit": ""
  },
  "ClarityAndFormatting": {
    "ClarityScore": 0,
    "FormattingScore": 0,
    "SpellingErrors": 0
  },
  "Summary": "",
  "Strengths": [],
  "AreasToImprove": [],
  "Tips": [],
  "CleanedResumeText": ""
}
```