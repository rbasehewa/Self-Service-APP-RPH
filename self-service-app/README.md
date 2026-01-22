# 🧪 Angular + Gemini AI – Self Service Demo

## 🎯 Why This Project?

The goal is to explore how **AI can enhance normal UI workflows**.

This project demonstrates:
- **Natural language data filtering**: Use Gemini AI to filter staff data with plain English queries
- **Smart table management**: AI-powered insights and analysis of staff data
- **Full-stack integration**: Angular frontend with dual backend architecture (.NET + Node.js)
- **Modern Angular patterns**: Signals, standalone components, and reactive programming

Example use case:

> An immunisation nurse wants to filter staff data using natural language:
> "Give me all non-immune staff born after 1990."
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
   - Processes natural language queries

2. **.NET Core Web API** (`Backend/StaffServiceAPI`)
   - Full REST API with PostgreSQL
   - Staff data management (CRUD operations)
   - Entity Framework Core with migrations
   - Comprehensive error handling and logging
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

## 🔐 Important Note — Why We Use Node/Express Proxy

The Node/Express server acts as a secure proxy for the Gemini AI API.

We use a tiny Node/Express server.js file because:

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

It only protects the API key — the actual business logic and data is handled by the .NET Core backend.

---

## 🗄️ .NET Core Backend

This project includes a **full-stack .NET Core backend** with PostgreSQL for staff data management.

### Features

- ✅ ASP.NET Core Web API (.NET 9)
- ✅ PostgreSQL database with Entity Framework Core
- ✅ Complete staff data management (CRUD operations)
- ✅ Database migrations and seeding (100 sample staff records)
- ✅ Comprehensive error handling and logging
- ✅ CORS configured for Angular frontend
- ✅ RESTful API design with proper HTTP status codes

### API Endpoints

**Staff Management:**
- `GET /api/staff` - Get all staff members
- `GET /api/staff/{id}` - Get staff by ID
- `GET /api/staff/immune-status/{status}` - Filter by immune status
- `POST /api/staff` - Create new staff member
- `PUT /api/staff/{id}` - Update staff member
- `DELETE /api/staff/{id}` - Delete staff member

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
- Seed 100 sample staff members with realistic data

### Database

View your data in pgAdmin:
1. Open pgAdmin
2. Navigate to: **Databases** → **StaffServiceDB** → **Tables** → **Staff**
3. Right-click **Staff** → **View/Edit Data** → **All Rows**

---

## 🎨 Frontend Features

### Staff Table Component
- **Real-time data loading** from .NET backend
- **AI-powered filtering** with natural language queries
- **Edit mode** with inline form validation
- **Sorting** by any column
- **Pagination** for large datasets
- **Immune status indicators** with color-coded badges
- **Responsive design** with modern UI

### Technologies Used
- Angular 19 with standalone components
- Signals for reactive state management
- Material Design styling
- TypeScript with strict type checking
- RxJS for async operations

---

## 🤖 AI Integration

### How It Works

1. User enters a natural language query (e.g., "Show me all non-immune staff")
2. Angular sends the query to Node/Express proxy
3. Proxy forwards to Gemini AI with context about the data structure
4. Gemini returns structured filter criteria
5. Angular applies filters to the staff table

### Example Queries

- "Show me all non-immune staff"
- "Find staff members born after 1990"
- "List immune staff in the cardiology department"
- "Show female staff who are non-immune"

---

## 📁 Project Structure

```
self-service-app/
├── src/
│   ├── app/
│   │   ├── components/
│   │   │   └── staff-table/          # Main staff table component
│   │   ├── models/
│   │   │   └── staff.model.ts        # Staff data model
│   │   ├── services/
│   │   │   ├── staff.service.ts      # Backend communication
│   │   │   └── ai.service.ts         # Gemini AI integration
│   │   └── app.component.ts          # Root component
│   └── environments/                  # Environment configs
├── Backend/
│   └── StaffServiceAPI/
│       ├── Controllers/
│       │   └── StaffController.cs    # API endpoints
│       ├── Data/
│       │   ├── AppDbContext.cs       # EF Core context
│       │   └── DataSeeder.cs         # Sample data generator
│       ├── Models/
│       │   └── Staff.cs              # Staff entity model
│       └── Program.cs                # API configuration
├── server.js                          # Gemini AI proxy
└── README.md
```