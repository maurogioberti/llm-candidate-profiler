# 📄 Resume Analysis Instructions

Take the following text extracted from a **Curriculum Vitae (Resume)** in PDF format.  
Process and evaluate it thoroughly, performing cleaning, rewriting, and a complete analysis according to the detailed criteria below.

---

## ✨ General Instructions

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

## 📋 General Information

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

## 🛠️ Skills Detected

For each skill:
- **SkillName**
- **SkillLevel**: Low / Medium / High / Very High.
- **Evidence**: text supporting the assessment.

---

## 🗝️ Keyword Coverage

- **KeywordsDetected**: list of detected keywords.
- **KeywordsMissing**: relevant keywords not found.
- **Density**: percentage of keyword density.
- **Context**: brief description of how the keywords appear.

---

## 🌍 Languages

For each language:
- **Language**
- **Proficiency**: Basic / Intermediate / Advanced / Native.
- **Evidence**: example text justifying the assessment.

---

## 📝 Clarity and Formatting

- **ClarityScore**: subjective evaluation (0–100).
- **FormattingScore**: structure and visual presentation (0–100).
- **SpellingErrors**: approximate count of spelling mistakes.

---

## 👔 Relevance to Target Role

- **TitleMatch**: degree of alignment with the target title.
- **ResponsibilityMatch**: degree of alignment with target responsibilities.
- **OverallFit**: High / Medium / Low.

---

## 🧠 Scoring

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

## 🧾 Summary and Feedback

- **Summary**: one-paragraph description of the candidate profile.
- **Strengths**: top 5 strengths.
- **AreasToImprove**: top 3 areas to improve.
- **Tips**: suggestions to improve the resume.

---

## 🧽 Cleaned Resume Text

Provide the cleaned, improved resume text ready for export.

---

## ✨ Example Output JSON

```json
{
  "GeneralInfo": {
    "Fullname": null,
    "TitleDetected": null,
    "TitlePredicted": null,
    "SeniorityLevel": null,
    "YearsExperience": null,
    "RelevantYears": null,
    "IndustryMatch": null,
    "TrajectoryPattern": null,
    "MainIndustry": null,
    "EnglishLevel": null,
    "OtherLanguages": [],
    "Location": null
  },
  "SkillMatrix": [],
  "KeywordCoverage": {
    "KeywordsDetected": [],
    "KeywordsMissing": [],
    "Density": null,
    "Context": null
  },
  "Languages": [],
  "Scores": {
    "GeneralScore": null,
    "ATSCompatibility": null,
    "ClarityScore": null,
    "FormattingScore": null,
    "KeywordDensity": null,
    "EnglishProficiency": null,
    "SeniorityMatch": null,
    "SkillCoverage": null,
    "AchievementsQuantification": null,
    "SoftSkillsCoverage": null
  },
  "Relevance": {
    "TitleMatch": null,
    "ResponsibilityMatch": null,
    "OverallFit": null
  },
  "ClarityAndFormatting": {
    "ClarityScore": null,
    "FormattingScore": null,
    "SpellingErrors": null
  },
  "Summary": null,
  "Strengths": [],
  "AreasToImprove": [],
  "Tips": [],
  "CleanedResumeText": null
}
```

## ✨ Example Output JSON DataType

```json
{
  "GeneralInfo": {
    "Fullname": "string",
    "TitleDetected": "string",
    "TitlePredicted": "string or null",
    "SeniorityLevel": "string or null",
    "YearsExperience": "number",
    "RelevantYears": "number",
    "IndustryMatch": "string or null",
    "TrajectoryPattern": "string or null",
    "MainIndustry": "string or null",
    "EnglishLevel": "string or null",
    "OtherLanguages": [
      {
        "Language": "string",
        "Proficiency": "string",
        "Evidence": "string"
      }
    ],
    "Location": "string or null"
  },
  "SkillMatrix": [
    {
      "SkillName": "string",
      "SkillLevel": "string",
      "Evidence": "string"
    }
  ],
  "KeywordCoverage": {
    "KeywordsDetected": ["string"],
    "KeywordsMissing": ["string"],
    "Density": "number",
    "Context": "string"
  },
  "Languages": [
    {
      "Language": "string",
      "Proficiency": "string",
      "Evidence": "string"
    }
  ],
  "Scores": {
    "GeneralScore": "number",
    "ATSCompatibility": "number",
    "ClarityScore": "number",
    "FormattingScore": "number",
    "KeywordDensity": "number",
    "EnglishProficiency": "number",
    "SeniorityMatch": "string",
    "SkillCoverage": "number",
    "AchievementsQuantification": "number",
    "SoftSkillsCoverage": "number"
  },
  "RelevanceToTargetRole": {
    "TitleMatch": "number",
    "ResponsibilityMatch": "number",
    "OverallFit": "number"
  },
  "ClarityAndFormatting": {
    "ClarityScore": "number",
    "FormattingScore": "number",
    "SpellingErrors": "number"
  },
  "Summary": "string",
  "Strengths": ["string"],
  "AreasToImprove": ["string"],
  "Tips": ["string"],
  "CleanedResumeText": "string"
}
```

---

## 📝 Additional Formatting Requirements

- **Fullname** must be a separate field from **TitleDetected**.
- **YearsExperience** and **RelevantYears** must be numeric integers (e.g., 5).
- **RelevanceToTargetRole** must contain:
  - Numeric percentages (0–100) for each match.
  - Optionally, a text label (Low/Moderate/High) in a separate property if needed.
- Always include **KeywordCoverage**, even if empty.
- All numeric fields must be integers without quotes.
- No empty strings; use `null` instead.

---

## 🧾 Summary and Feedback

- No trailing commas.
- All numeric fields must be numbers only (e.g., 5), not strings or dates.
- Use null if data is missing.
- Do not insert slashes or date formats in numeric fields.
- Respond ONLY with valid JSON.
- Ensure your JSON is syntactically correct.
- Do not include comments, explanations, or trailing characters after the final "}".

⚠️ Respond ONLY with a valid JSON object. Do not add any explanations or comments.

Below is the resume to process:

_@{PROCESSED_TEXT}