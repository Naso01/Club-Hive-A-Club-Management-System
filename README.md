# Club Hive

**Club Hive** is a web application for discovering and managing student clubs, built as a **club management system** with a Sheridan College–themed landing experience. The app lets visitors browse clubs, while **club executives** can register, sign in, create clubs (with optional images), and manage the clubs they lead.

---

## Team & process

This project was developed **over the course of a semester** by a **team of three** (including the repository owner). Work was tracked with **agile practices** using **Jira**, and the **software architecture** was modeled in **Visual Paradigm** before and during implementation.

---

## Features

| Area | Status |
|------|--------|
| Public club directory (`/Clubs`) | Implemented |
| Club executive registration | Implemented (`/Home/ClubExecutiveSignUp`) |
| Session-based login / logout (modal on layout) | Implemented |
| Account page for signed-in user | Implemented |
| Create / edit / delete clubs (executives only); image upload to `wwwroot/uploads/clubs` | Implemented (`ClubsController`) |
| “Manage Clubs” in nav for executives | Implemented |
| Role selection on generic Sign Up page (Student / Admin) | UI only (links not connected) |
| Events listing (`/Events` from home) | **Not implemented** — `Event` entity and DB table exist; no MVC surface yet |
| Password security | Plain-text comparison (demo / educational use only) |

---

## Tech stack

- **.NET 10** (`net10.0`) — ASP.NET Core **MVC**
- **Entity Framework Core 10** with **SQL Server** (LocalDB by default)
- **Razor views**, **Bootstrap 5**, **jQuery** (standard MVC template layout)
- **dotnet-ef** for migrations (pinned via `dotnet-tools.json`)

---

## Architecture (high level)

```text
Browser (Razor + Bootstrap/JS)
        │
        ▼
ASP.NET Core MVC (Controllers: Home, Clubs)
        │
        ▼
ApplicationDbContext (EF Core)
        │
        ▼
SQL Server (LocalDB / connection string)
```

- **Models**: `User` hierarchy (`Student`, `Admin`, `ClubExecutive`) using **table-per-hierarchy** on a single `Users` table with a `Discriminator` column; `Club` and `Event` with relationships configured in EF.
- **Persistence**: Migrations under `Migrations/`; `Program.cs` applies migrations on startup and includes a defensive SQL check for the `Rank` column on `Users`.
- **State**: **Distributed memory cache** + **session** (12-hour idle timeout) stores logged-in user id, first name, and rank string for UI and authorization checks.

---

## Repository layout

```text
Club Hive - A Club Management System/   # Main web project
  Controllers/                          # HomeController, ClubsController
  Data/                                 # ApplicationDbContext, DbInitializer (unused at startup)
  Models/                               # User, Club, Event, view models
  Views/                                # Razor pages for Home and Clubs
  Migrations/                           # EF Core migrations
  wwwroot/                              # CSS, JS, lib, uploads/clubs
  Program.cs, appsettings.json
SETUP.md                                # Local setup commands (authoritative for EF/database)
.config/dotnet-tools.json               # dotnet-ef tool manifest
```

---

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- **SQL Server LocalDB**, or change `ConnectionStrings:DefaultConnection` in `appsettings.json` to your SQL Server instance

---

## Setup & run

From the **repository root**, follow **[SETUP.md](SETUP.md)** for exact PowerShell commands. In short:

1. `dotnet restore` on the `.csproj`
2. `dotnet tool restore`
3. `dotnet tool run dotnet-ef database update` (after migrations exist)
4. `dotnet run --project ".\Club Hive - A Club Management System\Club Hive - A Club Management System.csproj"`

Default URLs are defined in `Properties/launchSettings.json` (e.g. **https://localhost:7060** and **http://localhost:5192**).

---

## Demo / seeded account

Migrations seed one **club executive** (see [SETUP.md](SETUP.md)):

- Email: `a@email.com`
- Password: `123`

**Warning:** Do not reuse real passwords; credentials are stored for demonstration.

---

## Development notes

- **EF tools**: Version is aligned with the EF Core packages in the `.csproj` (see `.config/dotnet-tools.json`).
- **Optional column migration**: `Program.cs` runs raw SQL to add `Users.Rank` if missing, for environments that predate that column.

---

## License / third-party

Third-party libraries (e.g. Bootstrap, jQuery) live under `wwwroot/lib/` with their respective licenses.

---

## Acknowledgments

Course project at **Sheridan College** context (branding/copy on the landing page). Built with standard ASP.NET Core MVC patterns and EF Core.
