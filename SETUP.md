# Club Hive Local Setup

## Prerequisites

- .NET SDK 10 installed
- SQL Server LocalDB (or update connection string to your SQL Server instance)

## First-time setup (run from repository root)

```powershell
dotnet restore ".\Club Hive - A Club Management System\Club Hive - A Club Management System.csproj"
dotnet tool restore
dotnet tool run dotnet-ef migrations add InitialCreate --project ".\Club Hive - A Club Management System\Club Hive - A Club Management System.csproj" --startup-project ".\Club Hive - A Club Management System\Club Hive - A Club Management System.csproj"
dotnet tool run dotnet-ef database update --project ".\Club Hive - A Club Management System\Club Hive - A Club Management System.csproj" --startup-project ".\Club Hive - A Club Management System\Club Hive - A Club Management System.csproj"
```

## After migrations are committed (most teammates)

```powershell
dotnet restore ".\Club Hive - A Club Management System\Club Hive - A Club Management System.csproj"
dotnet tool restore
dotnet tool run dotnet-ef database update --project ".\Club Hive - A Club Management System\Club Hive - A Club Management System.csproj" --startup-project ".\Club Hive - A Club Management System\Club Hive - A Club Management System.csproj"
```

## Run the app

```powershell
dotnet run --project ".\Club Hive - A Club Management System\Club Hive - A Club Management System.csproj"
```

## Seeded Club Executive

The database seed includes one `ClubExecutive` user:

- First Name: `Nathan`
- Last Name: `Serrano`
- Email: `a@email.com`
- Password: `123`
