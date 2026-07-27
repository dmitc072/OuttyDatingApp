using Microsoft.EntityFrameworkCore;
using Outty.Api.Data;
using Outty.Api.Services;
using Outty.Shared.Utilities;
using Azure.Identity;
using Google.Apis.Auth;

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


app.MapPost("/users/login", async (GoogleLoginRequest request, OuttyDbContext db, IConfiguration config) =>
{
    GoogleJsonWebSignature.Payload payload;

    try
    {
        var clientId = config["Authentication:Google:ClientId"];

        payload = await GoogleJsonWebSignature.ValidateAsync(
            request.IdToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = string.IsNullOrWhiteSpace(clientId) ? null : [clientId]
            });
    }
    catch (InvalidJwtException)
    {
        return Results.Unauthorized();
    }

    var user = await db.Users
        .Include(u => u.Profile)
        .FirstOrDefaultAsync(u => u.Email == payload.Email);

    if (user is null)
    {
        user = new User { Email = payload.Email, CreatedAtUtc = DateTime.UtcNow };
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }

    return Results.Ok(new
    {
        UserId = user.Id,
        user.Email,
        HasProfile = user.Profile is not null
    });
})
.WithName("GoogleLogin");

app.MapPost("/profiles", async (CreateProfileRequest request, OuttyDbContext db) =>
{
    var userExists = await db.Users.AnyAsync(u => u.Id == request.UserId);
    if (!userExists)
    {
        return Results.BadRequest("UserId does not exist.");
    }

    var alreadyHasProfile = await db.Profiles.AnyAsync(p => p.UserId == request.UserId);
    if (alreadyHasProfile)
    {
        return Results.Conflict("This user already has a profile.");
    }

    var state = await db.States.FirstOrDefaultAsync(s => s.Abbreviation == request.State);
    if (state is null)
    {
        return Results.BadRequest($"Unknown state: {request.State}");
    }

    var profile = new Profile
    {
        UserId = request.UserId,
        DisplayName = request.DisplayName,
        BirthDate = request.BirthDate,
        City = request.City,
        State = request.State,
        ZipCode = request.ZipCode,
        Pronouns = request.Pronouns,
        Bio = request.Bio,
        PreferredDistance = request.PreferredDistance,
        SearchRadiusMiles = SearchRadiusValidator.Normalize(request.SearchRadiusMiles),
        CreatedAtUtc = DateTime.UtcNow,
        UpdatedAtUtc = DateTime.UtcNow
    };

    foreach (var interestRequest in request.Interests)
    {
        var interest = await db.Interests.FirstOrDefaultAsync(i => i.Name == interestRequest.Name);
        var experienceLevel = await db.ExperienceLevels.FirstOrDefaultAsync(e => e.ExperienceLevel1 == interestRequest.ExperienceLevel);

        if (interest is null || experienceLevel is null)
        {
            return Results.BadRequest($"Unknown interest or experience level: {interestRequest.Name} / {interestRequest.ExperienceLevel}");
        }

        profile.ProfileInterests.Add(new ProfileInterest
        {
            Interest = interest,
            ExperienceLevel = experienceLevel
        });
    }

    foreach (var goalName in request.Goals)
    {
        var goal = await db.Goals.FirstOrDefaultAsync(g => g.Name == goalName);

        if (goal is null)
        {
            return Results.BadRequest($"Unknown goal: {goalName}");
        }

        profile.Goals.Add(goal);
    }

    db.Profiles.Add(profile);
    await db.SaveChangesAsync();

    return Results.Created($"/profiles/{profile.Id}", new { profile.Id });
})
.WithName("CreateProfile");

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

record GoogleLoginRequest(string IdToken);

record InterestSelection(string Name, string ExperienceLevel);

record CreateProfileRequest(
    int UserId,
    string DisplayName,
    DateOnly BirthDate,
    string City,
    string State,
    string ZipCode,
    string? Pronouns,
    string? Bio,
    string PreferredDistance,
    int SearchRadiusMiles,
    List<InterestSelection> Interests,
    List<string> Goals);

