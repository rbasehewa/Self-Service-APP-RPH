# 🧪 Angular + Gemini AI – Self Service Demo

## 🎯 Why This Project?

The goal is to explore how **AI can enhance normal UI workflows**.

Example use case:

> An immunisation nurse wants to filter staff data using natural language:
> “Give me all non-immune staff born after 1990.”
> Instead of writing filters manually, Gemini returns structured JSON results.

You can also generate free text or HTML using prompts.

![sample UI](https://github.com/rbasehewa/Self-Service-APP-RPH/blob/main/self-service-app/public/img/sampleUI.png)

---

## 🏗️ Architecture

### Full Stack Architecture

```
Angular App (UI + Signals)
        │
        ├─────────────────────┐
        │                     │
        ▼                     ▼
Node/Express Proxy      .NET Core API
(Gemini AI)             (PostgreSQL)
  • Hides API key         • User CRUD
  • No logic              • EF Core
        │                     │
        ▼                     ▼
Google Gemini API       PostgreSQL DB
```

### Two Backends

1. **Node/Express Proxy** (`server.js`)
   - Secures Google Gemini API key
   - No business logic
   - Handles AI requests only

2. **.NET Core Web API** (`Backend/StaffServiceAPI`)
   - Full REST API with PostgreSQL
   - User management (CRUD operations)
   - Entity Framework Core
   - For learning backend concepts

#### Flow

- **AI Features**: Angular → Node Proxy → Gemini → Angular
- **User Data**: Angular → .NET API → PostgreSQL → Angular

---

## ▶️ How to Run

### Prerequisites

- Node.js and npm
- .NET 9 SDK
- PostgreSQL (for the .NET backend)

### Quick Start

1. **Install Angular dependencies**
   ```bash
   npm install
   ```

2. **Run Angular frontend**
   ```bash
   ng serve
   ```
   Runs on: `http://localhost:4200`

3. **Run Node/Express proxy** (for Gemini AI features)
   ```bash
   node server.js
   ```
   Runs on: `http://localhost:3000`

4. **Run .NET Core backend** (for user data)
   ```bash
   cd Backend/StaffServiceAPI
   dotnet run
   ```
   Runs on: `http://localhost:5107`

   See [Backend/README.md](Backend/README.md) for detailed setup instructions.

### Running Both Backends

You'll need **3 terminal windows**:
- Terminal 1: `ng serve` (Angular)
- Terminal 2: `node server.js` (Gemini proxy)
- Terminal 3: `cd Backend/StaffServiceAPI && dotnet run` (.NET API)

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

---

## 🗄️ .NET Core Backend (New!)

This project now includes a **full-stack .NET Core backend** with PostgreSQL for learning purposes.

### Features

- ✅ ASP.NET Core Web API (.NET 9)
- ✅ PostgreSQL database with Entity Framework Core
- ✅ CRUD operations for user management
- ✅ Database migrations and seeding (100 sample users)
- ✅ Comprehensive error handling and logging
- ✅ CORS configured for Angular frontend

### API Endpoints

- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/immune-status/{status}` - Filter by immune status
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Setup

See the complete setup guide in [Backend/README.md](Backend/README.md)

**Quick setup:**
```bash
cd Backend/StaffServiceAPI
dotnet run
```

The API will automatically:
- Create the PostgreSQL database
- Apply migrations
- Seed 100 sample users

### Database

View your data in pgAdmin:
1. Open pgAdmin
2. Navigate to: **Databases** → **StaffServiceDB** → **Tables** → **Users**
3. Right-click **Users** → **View/Edit Data** → **All Rows**