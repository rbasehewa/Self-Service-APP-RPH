// Load environment variables (like API key) from a .env file
require('dotenv').config(); 
const express = require('express');
const cors = require('cors');
const { GoogleGenAI } = require('@google/genai');

// Use environment variable for the API Key
const GEMINI_API_KEY = process.env.GEMINI_API_KEY; 

// Initialize the GoogleGenAI client
if (!GEMINI_API_KEY) {
    console.error("Error: GEMINI_API_KEY is not set in environment variables.");
    process.exit(1);
}
const ai = new GoogleGenAI({ apiKey: GEMINI_API_KEY });
const app = express();
const port = 3000;

// Middleware Setup
// 1. Enable CORS for Angular frontend (or any origin)
app.use(cors({
    origin: '*', // Allow all origins for simplicity in development
    methods: ['GET', 'POST'],
}));

// 2. Enable Express to parse JSON body from incoming requests
app.use(express.json()); 

// --- API Route ---
// This is the POST endpoint that your Angular service calls.
app.post('/generate-content', async (req, res) => {
    // The prompt is expected in the request body from Angular
    const { prompt } = req.body; 

    if (!prompt) {
        return res.status(400).send({ error: 'Prompt is required.' });
    }

    try {
        console.log(`Received prompt: "${prompt.substring(0, 50)}..."`);
        
        // Call the Gemini API
        const response = await ai.models.generateContent({
            model: "gemini-2.5-flash", 
            contents: [{ role: "user", parts: [{ text: prompt }] }],
        });

        // Send the raw response back to the Angular frontend
        res.json(response);

    } catch (error) {
        console.error('Gemini API Error:', error.message);
        res.status(500).json({ 
            error: 'Failed to generate content from Gemini API.',
            details: error.message
        });
    }
});

// --- Server Start ---
app.listen(port, () => {
    console.log(`Express server listening at http://localhost:${port}`);
    console.log('Ready to receive POST requests on /generate-content');
});

// IMPORTANT: Add this simple GET handler for the root path so users know it's running.
app.get('/', (req, res) => {
    res.send('Gemini Proxy Server is running. The client app should use the POST /generate-content endpoint.');
});