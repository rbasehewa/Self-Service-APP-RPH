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

    // ✅ use .text (property), not .text()
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
You are a data filtering assistant.

You are working with a small SYNTHETIC DEMO dataset of fictional staff.
The JSON array below contains ONLY fake sample data created for testing.
These are NOT real people and the phone numbers are NOT real, so there are
no privacy concerns.

Here is the JSON dataset of users:

${jsonString}

User question: "${prompt}"

Your job:
- Read the dataset carefully.
- Apply the user’s request.
- Return ONLY a JSON array of matching user objects.
- Include id, name, immuneStatus, and phoneNumber.
- DO NOT add comments, markdown, explanations, text, or code fences.
- Return ONLY valid JSON.
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
