// Load environment variables (like API key) from a .env file
require('dotenv').config();
const express = require('express');
const cors = require('cors');
const { GoogleGenAI } = require('@google/genai');

const GEMINI_API_KEY = process.env.GEMINI_API_KEY;

if (!GEMINI_API_KEY) {
  console.error('Error: GEMINI_API_KEY is not set in environment variables.');
  process.exit(1);
}

const ai = new GoogleGenAI({ apiKey: GEMINI_API_KEY });

const app = express();
const port = 3000;

// -----------------------------
// Middleware
// -----------------------------
app.use(cors({ origin: '*', methods: ['GET', 'POST'] }));
app.use(express.json());

// -----------------------------
// Generate: general text / HTML
// -----------------------------
app.post('/generate', async (req, res) => {
  const { prompt } = req.body;

  if (!prompt) {
    return res.status(400).json({ error: 'Prompt is required.' });
  }

  try {
    const response = await ai.models.generateContent({
      model: 'gemini-2.5-flash',
      contents: [{ role: 'user', parts: [{ text: prompt }] }],
    });

    // use .text (property), not .text()
    const text = response.text || '';
    return res.json({ text });
  } catch (error) {
    console.error('Gemini /generate error:', error);
    return res.status(500).json({ error: 'Gemini error', details: error.message });
  }
});

// -----------------------------
// Query Users (AI Filters JSON)
// -----------------------------
app.post('/query-users', async (req, res) => {
  const { prompt, data } = req.body;

  if (!prompt || !data) {
    return res.status(400).json({ error: 'Prompt and data are required.' });
  }

  try {
    const jsonString = JSON.stringify(data, null, 2);

    const promptTemplate = `
You are a **strict data filtering assistant**.

You are given a JSON array called "users". 
Each user object has these exact fields:
- id (number)
- name (string)
- immuneStatus (one of "Immune", "Non-Immune", "Unknown")
- yearOfBirth (number, e.g. 1988)
- phoneNumber (string)
- vaccineDate (string or null)

VERY IMPORTANT RULES:
- You MUST treat the JSON data as the single source of truth.
- You MUST NOT invent, modify, or guess any values.
- You MUST NOT create new users.
- You MUST NOT change immuneStatus, yearOfBirth or phoneNumber.
- If the user asks for filters (e.g. "non immune", "after 1990", "older than 40"),
  you MUST apply ALL of those conditions exactly.

Examples of correct behaviour:

1) If the user says: "Give me non immune users born after 1990."
   - Only return users where immuneStatus === "Non-Immune"
     AND yearOfBirth > 1990.

2) If the user says: "Show immune users with their vaccineDate and phoneNumber."
   - Only return users where immuneStatus === "Immune".
   - You may keep the full objects, but they **must** come from the dataset.

3) If the question is unrelated to this dataset (e.g. "What is a dog?")
   - Return an **empty array**: [].

Here is the JSON dataset of users:

${jsonString}

User question: "${prompt}"

Your job:
- Carefully read the question.
- Apply the filters to the dataset.
- Return ONLY a JSON array of matching user objects from the dataset.
- Include full objects (id, name, immuneStatus, yearOfBirth, phoneNumber, vaccineDate).
- Do NOT include any comments, explanations, markdown or backticks.
- If there are no matches or the question is not about this dataset, return [].
`.trim();

    const response = await ai.models.generateContent({
      model: 'gemini-2.5-flash',
      contents: [{ role: 'user', parts: [{ text: promptTemplate }] }],
    });

    const text = response.text || '';

    try {
      const result = JSON.parse(text);
      return res.json({ result });
    } catch (parseError) {
      return res.json({ raw: text });
    }
  } catch (error) {
    console.error('Gemini query-users error:', error);
    return res.status(500).json({
      error: 'Failed to query users.',
      details: error.message,
    });
  }
});

// -----------------------------
// Root
// -----------------------------
app.get('/', (req, res) => {
  res.send('Gemini Proxy Server is running. Use POST /generate or /query-users.');
});

// -----------------------------
// Start Server
// -----------------------------
app.listen(port, () => {
  console.log(`Express server listening at http://localhost:${port}`);
  console.log('Ready on /generate and /query-users');
});
