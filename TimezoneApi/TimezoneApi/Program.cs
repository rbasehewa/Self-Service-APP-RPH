// Bring in EF Core types like DbContext, UseNpgsql, LINQ async methods
using Microsoft.EntityFrameworkCore;

// Bring in our own DbContext (data access layer) and entity model
using TimezoneApi.Data;    // <- file: Data/AppDbContext.cs
using TimezoneApi.Models;  // <- file: Models/EventRecord.cs


// Create the host builder (loads appsettings.json, env vars, etc.)
var builder = WebApplication.CreateBuilder(args);

var MyCors = "_myCors";
builder.Services.AddCors(o => o.AddPolicy(MyCors, p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Read & validate connection string early (clear error if missing)
var cs = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(cs))
    throw new InvalidOperationException("Missing ConnectionStrings:Default in appsettings.json");

// Register EF Core + Npgsql ONCE
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));


// ------------------------------------
// SERVICES (Dependency Injection setup)
// ------------------------------------


// Add OpenAPI/Swagger services so we get a nice UI to test endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build the WebApplication (after this, we register middleware and endpoints)
var app = builder.Build();

// --------------------
// MIDDLEWARE pipeline
// --------------------

// Enable Swagger JSON endpoint and the interactive Swagger UI at /swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(MyCors);
// --------------------------------
// Minimal API endpoints (no MVC)
// --------------------------------

// GET /api/events  -> returns a list of saved events
app.MapGet("/api/events", async (AppDbContext db) =>
{
    // Query the database using EF Core LINQ:
    // - order newest first by SavedUtc
    // - project each record into a lightweight anonymous object for the API response
    var list = await db.Events
        .OrderByDescending(e => e.SavedUtc)
        .Select(e => new
        {
            id = e.Id,                                           // GUID id
            title = e.Title,                                     // string title
            savedUtcIso = e.SavedUtc.ToUniversalTime().ToString("o"), // ISO 8601 UTC string
            savedTz = e.SavedTz                                   // e.g., "Australia/Sydney"
        })
        .ToListAsync();

    // Return HTTP 200 OK with the list
    return Results.Ok(list);
});

// POST /api/events -> inserts a new row with "now" in UTC and tags with Sydney TZ
app.MapPost("/api/events", async (AppDbContext db) =>
{
    // Create a new entity (C# object) that matches one row in app.events
    var entity = new EventRecord
    {
        Title = "Saved “now” in Sydney",  // demo title (you can accept from request body later)
        SavedUtc = DateTimeOffset.UtcNow,    // authoritative instant in UTC
        SavedTz = "Australia/Sydney"        // tag with IANA timezone for original context
    };

    // Track the new entity so EF will insert it on SaveChanges
    db.Events.Add(entity);

    // Write changes to the database (INSERT)
    await db.SaveChangesAsync();

    // Return what we inserted (id + nicely formatted UTC string + saved TZ)
    return Results.Ok(new
    {
        id = entity.Id,
        title = entity.Title,
        savedUtcIso = entity.SavedUtc.ToUniversalTime().ToString("o"),
        savedTz = entity.SavedTz
    });
});



// Start the web app (blocks here and begins listening on the configured port)
app.Run();
