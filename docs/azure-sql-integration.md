# Azure SQL Integration — What Was Done

## 1. Azure resources

Everything lives in resource group `Outty-RG` (region `centralus`):

| Resource        | Name               | Notes                                                                            |
| --------------- | ------------------ | -------------------------------------------------------------------------------- |
| SQL Server      | `outty-sql-srv1`   | admin login `outtyadmin`; firewall open to the dev machine's IP + Azure services |
| SQL Database    | `outty-db`         | General Purpose, Serverless, auto-pauses after 60 min idle                       |
| Storage Account | `outtystorageacct` | StorageV2, Standard_LRS, public blob access disabled                             |
| Blob Container  | `profile-photos`   | for profile photo uploads (not wired into code yet)                              |

## 2. Local credentials — `dotnet user-secrets`

The SQL and storage connection strings are **not** in `appsettings.json` (that's committed to git). They're stored locally, outside the repo, via .NET's user-secrets:

- Project is linked to a secrets store via `<UserSecretsId>` in [Outty.Api.csproj](../src/Outty.Api/Outty.Api.csproj) (added by `dotnet user-secrets init`).
- Actual values live in `~/.microsoft/usersecrets/<UserSecretsId>/secrets.json` on each developer's machine.
- Keys used: `ConnectionStrings:OuttyDb` and `ConnectionStrings:OuttyStorage`.

To view/set them yourself:

```bash
cd src/Outty.Api
dotnet user-secrets list
dotnet user-secrets set "ConnectionStrings:OuttyDb" "<connection-string>"
```

Every teammate needs to run `dotnet user-secrets set` on their own machine with the shared credential.

## 3. Connecting from VS Code

Used the **SQL Server (mssql)** extension's "Connect to Database" wizard:

- Input type: **Browse Azure** → signed in with the Azure account → picked subscription "Azure for Students" → expanded `outty-sql-srv1` → selected `outty-db`.
- Authentication Type: **SQL Login** → username `outtyadmin`, password from user-secrets → saved to the OS keychain.
- From there, queries were run directly against `outty-db` via **New Query** on the saved connection.

## 4. Database schema

The tables were hand-written and run directly in the VS Code query editor — this project does **not** use EF Core migrations to create the schema.

Source of truth: [db/schema.sql](../db/schema.sql) — `Users`, `States`, `Profiles`, `ProfilePhotos`, `Interests`, `ExperienceLevel`, `ProfileInterests`, `Goals`, `ProfileGoals`. Whenever the live schema changes, that file should be updated to match.

## 5. NuGet packages added to Outty.Api

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
```

Both are in [Outty.Api.csproj](../src/Outty.Api/Outty.Api.csproj). `.Design` is a build-time-only dependency (`PrivateAssets=all`) — it powers the `dotnet ef` tooling but isn't shipped in the running app.

Also installed globally (one-time, per machine): `dotnet tool install --global dotnet-ef`.

## 6. `src/Outty.Api/Data/` — scaffolded from the live database

Rather than hand-writing C# model classes and letting EF Core generate the schema (**code-first**), this project went the other direction — **database-first**: the schema already existed, so the C# classes were generated _from_ it.

```bash
dotnet ef dbcontext scaffold "Name=ConnectionStrings:OuttyDb" Microsoft.EntityFrameworkCore.SqlServer \
  -o Data -c OuttyDbContext --context-dir Data --no-onconfiguring --force
```

- `"Name=ConnectionStrings:OuttyDb"` — tells EF to pull the connection string from configuration (user-secrets) at scaffold time, so the raw connection string (with password) never has to be typed into a command or a file.
- `--no-onconfiguring` — without this flag, EF bakes the connection string as plain text directly into the generated `OuttyDbContext.cs`, which would then get committed to git. This flag forces the context to require a `DbContextOptions` instead, wired up in `Program.cs`.
- `--force` — overwrite the previously generated files when re-running after a schema change.

What got generated in [src/Outty.Api/Data/](../src/Outty.Api/Data):

| File                 | Maps to table          | Notes                                                                                                                                                           |
| -------------------- | ---------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `OuttyDbContext.cs`  | —                      | the `DbContext`; `DbSet<T>` per table + all relationships (`OnModelCreating`)                                                                                   |
| `User.cs`            | `dbo.Users`            |                                                                                                                                                                 |
| `State.cs`           | `dbo.States`           |                                                                                                                                                                 |
| `Profile.cs`         | `dbo.Profiles`         | has nav properties to `User`, `State`, `ProfilePhotos`, `ProfileInterests`, `Goals`                                                                             |
| `ProfilePhoto.cs`    | `dbo.ProfilePhotos`    |                                                                                                                                                                 |
| `Interest.cs`        | `dbo.Interests`        |                                                                                                                                                                 |
| `ExperienceLevel.cs` | `dbo.ExperienceLevel`  | property is named `ExperienceLevel1` — the table's column is named the same as the table itself, which EF can't map to a same-named property, so it appends `1` |
| `ProfileInterest.cs` | `dbo.ProfileInterests` | join table with an extra column (`ExperienceLevelId`), so it gets a real entity class instead of being folded into a pure many-to-many                          |
| `Goal.cs`            | `dbo.Goals`            | `ProfileGoals` has no extra columns, so it does **not** get its own class — EF represents it as a direct many-to-many between `Profile` and `Goal` instead      |

**Re-running this command is how the C# models get updated any time the schema in Azure changes.** Manually editting the generated files will be overwritten next time.

## 7. `Program.cs`

Two additions:

```csharp
using Microsoft.EntityFrameworkCore;
using Outty.Api.Data;

builder.Services.AddDbContext<OuttyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OuttyDb")));
```

Registers `OuttyDbContext` in dependency injection. `GetConnectionString("OuttyDb")` reads `ConnectionStrings:OuttyDb` from configuration — in Development that resolves to user-secrets automatically (no code needed for that part, it's built into `WebApplication.CreateBuilder`).

```csharp
app.MapGet("/states", async (OuttyDbContext db) =>
{
    var states = await db.States
        .OrderBy(s => s.Name)
        .Select(s => new { s.Abbreviation, s.Name })
        .ToListAsync();

    return states;
})
.WithName("GetStates");
```

A minimal endpoint added purely to prove the whole chain works — queries `dbo.States` through EF Core and returns it as JSON. Verified locally: running the API and hitting `GET /states` returned all 51 rows (50 states + DC) live from Azure SQL. This isn't meant to be a permanent endpoint — swap it out once real feature endpoints (profiles, matches, etc.) are being built.

## 8. Running it locally

```bash
cd src/Outty.Api
dotnet run
curl http://localhost:5157/states   # or whatever port launchSettings.json assigns
```

Needs: `dotnet user-secrets` populated with `ConnectionStrings:OuttyDb`, and the dev machine's public IP allowed through the SQL server firewall (VS Code / the app will prompt to add it automatically on a connection failure).
