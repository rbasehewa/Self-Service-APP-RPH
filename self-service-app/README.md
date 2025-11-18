# 🧪 Angular + Gemini AI – Self Service Demo

## 🎯 Why This Project?

The goal is to explore how **AI can enhance normal UI workflows**.

Example use case:

> An immunisation nurse wants to filter staff data using natural language:
> “Give me all non-immune users born after 1990.”
> Instead of writing filters manually, Gemini returns structured JSON results.

You can also generate free text or HTML using prompts.

---

## 🏗️ Architecture (Simple Diagram)

Angular App (UI + Signals)
        │
        ▼
 Node/Express Proxy (server.js)
  • No business logic
  • Only hides the API key
        │
        ▼
 Google Gemini API

#### Flow

Angular → Node Proxy → Gemini → Node Proxy → Angular UI

---

## ▶️ How to Run

1. Install dependencies

`npm install`

2. Run Angular

`ng serve`

3. Run backend server

`node server.js`

- 3.1. Server runs on:

  `http://localhost:3000`

- 3.2. Angular runs on::

  `http://localhost:4200`

---

## 🔐 Important Note — Why We Use Node/Express

This project does NOT have a real backend.

We only use a tiny Node/Express server.js file because:

❗ You must NEVER expose an API key in Angular

If you put your Google Gemini key inside Angular:

  - Anyone can open DevTools → Network → find your key

  - They can use it

 - You get charged

 - Your quota gets abused

 - Your project is compromised

✔️ So the Node server acts as a secure proxy

The proxy:

 - Accepts requests from Angular

 - Adds the API key securely (from .env)

 - Forwards the request to Gemini

 - Returns the response to Angular

It only protects the API key — it is NOT a backend application.