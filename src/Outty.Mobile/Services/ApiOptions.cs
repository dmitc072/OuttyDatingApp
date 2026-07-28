namespace Outty.Mobile.Services;

// Points at the real deployed API (Azure App Service). For local-only development
// against `dotnet run --project src/Outty.Api`, temporarily swap this to
// "http://10.0.2.2:5157" (Android emulator) or "http://localhost:5157" (Windows/other).
public static class ApiOptions
{
    public const string BaseAddress = "https://outty-api.azurewebsites.net";
}
