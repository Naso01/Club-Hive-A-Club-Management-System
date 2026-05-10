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
