# Staff Service API - .NET Core Backend

This is a .NET 9 Web API backend for the Staff Service application, using PostgreSQL as the database.

## Prerequisites

- .NET 9 SDK
- PostgreSQL installed and running
- pgAdmin (optional, for database management)

## Database Configuration

The connection string is in `StaffServiceAPI/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=StaffServiceDB;Username=postgres;Password=postgres"
}
```

**Update the username and password** to match your PostgreSQL credentials if different.

## Running the API

1. **Navigate to the project directory:**
   ```bash
   cd Backend/StaffServiceAPI
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

   The API will start on:
   - HTTPS: `https://localhost:7000` (or check the console output)
   - HTTP: `http://localhost:5000` (or check the console output)

3. **Database will be created automatically:**
   - On first run, the app will create the database `StaffServiceDB`
   - Apply migrations
   - Seed 100 sample users

## Available Endpoints

### GET /api/users
Get all users.

**Example:**
```bash
curl https://localhost:7000/api/users
```

### GET /api/users/{id}
Get a specific user by ID.

**Example:**
```bash
curl https://localhost:7000/api/users/1
```

### GET /api/users/immune-status/{status}
Filter users by immune status ("Immune", "Non-Immune", or "Unknown").

**Example:**
```bash
curl https://localhost:7000/api/users/immune-status/Immune
```

### POST /api/users
Create a new user.

**Example:**
```bash
curl -X POST https://localhost:7000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "immuneStatus": "Immune",
    "phoneNumber": "0400 200 101",
    "yearOfBirth": 1990,
    "vaccineDate": "2024-01-15"
  }'
```

### PUT /api/users/{id}
Update an existing user.

**Example:**
```bash
curl -X PUT https://localhost:7000/api/users/1 \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "name": "Alice Adams Updated",
    "immuneStatus": "Immune",
    "phoneNumber": "0400 100 001",
    "yearOfBirth": 1988,
    "vaccineDate": "2023-03-15"
  }'
```

### DELETE /api/users/{id}
Delete a user by ID.

**Example:**
```bash
curl -X DELETE https://localhost:7000/api/users/1
```

## User Model

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ImmuneStatus { get; set; } // "Immune", "Non-Immune", or "Unknown"
    public string PhoneNumber { get; set; }
    public int YearOfBirth { get; set; }
    public string VaccineDate { get; set; } // 'YYYY-MM-DD' or 'N/A'
}
```

## Debugging

### Using Visual Studio
1. Open `StaffServiceAPI.csproj` in Visual Studio
2. Set breakpoints in `Controllers/UsersController.cs`
3. Press F5 to run with debugging
4. Use tools like Postman or curl to hit the endpoints

### Using VS Code
1. Open the Backend folder in VS Code
2. Install the C# extension
3. Set breakpoints in `Controllers/UsersController.cs`
4. Press F5 and select ".NET Core Launch (web)"

### Database Debugging
1. Open pgAdmin
2. Connect to your PostgreSQL server
3. Navigate to Databases > StaffServiceDB > Schemas > public > Tables > Users
4. Right-click on Users > View/Edit Data > All Rows

## Migrations

### Create a new migration
```bash
dotnet ef migrations add MigrationName
```

### Apply migrations
```bash
dotnet ef database update
```

### Remove last migration
```bash
dotnet ef migrations remove
```

## Project Structure

```
StaffServiceAPI/
├── Controllers/
│   └── UsersController.cs       # API endpoints
├── Models/
│   └── User.cs                  # User entity
├── Data/
│   ├── AppDbContext.cs          # EF Core context
│   ├── DataSeeder.cs            # Seeds 100 users
│   └── Migrations/              # EF Core migrations
├── Program.cs                   # App configuration
└── appsettings.json             # Configuration
```

## CORS Configuration

The API is configured to accept requests from the Angular frontend at `http://localhost:4200`.

If you need to change this, edit `Program.cs`:

```csharp
policy.WithOrigins("http://localhost:4200")
```

## Troubleshooting

### PostgreSQL Connection Issues
1. Check PostgreSQL is running: Open pgAdmin or run `psql` in terminal
2. Verify connection string credentials in `appsettings.json`
3. Check PostgreSQL port (default: 5432)

### Migration Issues
If migrations fail, you can manually drop the database in pgAdmin and restart the app.

### Port Already in Use
If the default port is taken, edit `Properties/launchSettings.json` to change the port.

## Learning Resources

- **Entity Framework Core**: https://learn.microsoft.com/en-us/ef/core/
- **ASP.NET Core Web API**: https://learn.microsoft.com/en-us/aspnet/core/web-api/
- **PostgreSQL with EF Core**: https://www.npgsql.org/efcore/
