# ✂️ Snip — URL Shortener

A full-stack URL shortener built with **ASP.NET Core 8** and **Entity Framework Core** (SQLite).

## Features

- 🔗 Shorten any valid `http/https` URL
- 📊 Click tracking per link
- ⏳ Optional expiry (set TTL in days)
- 🔄 Duplicate detection — same URL always returns the same short code
- 🗑️ Delete links
- 📖 Swagger UI for API exploration
- 🖥️ Clean frontend served from `wwwroot`

## Tech Stack

| Layer    | Tech                          |
|----------|-------------------------------|
| Backend  | ASP.NET Core 8 Web API        |
| ORM      | Entity Framework Core 8       |
| Database | SQLite (via EF Core)          |
| Frontend | Vanilla HTML/CSS/JS           |
| API Docs | Swagger / OpenAPI             |

## Project Structure

```
UrlShortener/
├── Controllers/
│   └── UrlController.cs       # REST API endpoints
├── Data/
│   └── AppDbContext.cs        # EF Core DbContext
├── Models/
│   ├── ShortUrl.cs            # Entity model
│   └── Dtos.cs                # Request/response DTOs
├── Services/
│   ├── IUrlService.cs         # Service interface
│   └── UrlService.cs          # Business logic
├── wwwroot/
│   └── index.html             # Frontend UI
└── Program.cs                 # App setup & middleware
```

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Run

```bash
cd UrlShortener
dotnet restore
dotnet run
```

Open: [http://localhost:5000](http://localhost:5000)  
Swagger: [http://localhost:5000/swagger](http://localhost:5000/swagger)

## API Endpoints

| Method | Endpoint              | Description          |
|--------|-----------------------|----------------------|
| POST   | `/api/urls`           | Shorten a URL        |
| GET    | `/api/urls`           | List all short URLs  |
| GET    | `/api/urls/{code}/stats` | Get stats for a code |
| DELETE | `/api/urls/{code}`    | Delete a short URL   |
| GET    | `/{code}`             | Redirect to original |

### Example Request

```bash
curl -X POST http://localhost:5000/api/urls \
  -H "Content-Type: application/json" \
  -d '{"originalUrl": "https://github.com", "expiresInDays": 30}'
```

### Example Response

```json
{
  "shortUrl": "http://localhost:5000/aB3xYz",
  "code": "aB3xYz",
  "originalUrl": "https://github.com",
  "clicks": 0,
  "createdAt": "2024-01-15T10:30:00Z",
  "expiresAt": "2024-02-14T10:30:00Z"
}
```

## Resume Talking Points

- Designed and implemented a **RESTful API** using ASP.NET Core 8 with clean separation of concerns (Controllers → Services → Data)
- Used **Entity Framework Core** with SQLite for ORM-based persistence including migrations and unique constraints
- Implemented **URL validation**, duplicate detection, and **TTL-based expiry** logic in the service layer
- Built **click tracking** with atomic DB updates on every redirect
- Served a **static frontend** via `wwwroot` with Swagger UI for API documentation
- Applied **dependency injection** and **interface-based service design** for testability
