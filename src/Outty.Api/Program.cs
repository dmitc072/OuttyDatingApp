using Microsoft.EntityFrameworkCore;
using Outty.Api.Data;
using Outty.Api.Services;
using Outty.Shared.Utilities;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddAzureKeyVault(
    new Uri("https://outty-kv.vault.azure.net/"),
    new DefaultAzureCredential());

builder.Services.AddOpenApi();

builder.Services.AddDbContext<OuttyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OuttyDb")));

builder.Services.AddScoped<MatchingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("/states", async (OuttyDbContext db) =>
{
    var states = await db.States
        .OrderBy(s => s.Name)
        .Select(s => new { s.Abbreviation, s.Name })
        .ToListAsync();

    return states;
})
.WithName("GetStates");

app.MapGet("/matches/candidates/{profileId:int}", async (int profileId, MatchingService matchingService) =>
{
    var candidates = await matchingService.GetCandidatesAsync(profileId);
    return candidates;
})
.WithName("GetMatchCandidates");

app.MapPut("/profiles/{profileId:int}/search-radius", async (int profileId, SearchRadiusRequest request, OuttyDbContext db) =>
{
    var profile = await db.Profiles.FindAsync(profileId);
    if (profile is null)
    {
        return Results.NotFound();
    }

    profile.SearchRadiusMiles = SearchRadiusValidator.Normalize(request.Miles);
    profile.UpdatedAtUtc = DateTime.UtcNow;
    await db.SaveChangesAsync();

    return Results.Ok(new { profile.Id, profile.SearchRadiusMiles });
})
.WithName("SetSearchRadius");

app.Run();

record SearchRadiusRequest(int? Miles);

