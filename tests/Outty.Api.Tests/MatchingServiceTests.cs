using Microsoft.EntityFrameworkCore;
using Outty.Api.Data;
using Outty.Api.Services;

namespace Outty.Api.Tests;

public class MatchingServiceTests
{
    private static OuttyDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<OuttyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static User AddUser(OuttyDbContext db, string email)
    {
        var user = new User { Email = email, CreatedAtUtc = DateTime.UtcNow };
        db.Users.Add(user);
        return user;
    }

    private static Profile AddProfile(
        OuttyDbContext db,
        User user,
        string displayName,
        string state,
        params byte[] interestIds)
    {
        var profile = new Profile
        {
            User = user,
            DisplayName = displayName,
            BirthDate = new DateOnly(2000, 1, 1),
            City = "Anytown",
            State = state,
            ZipCode = "30144",
            PreferredDistance = "Within 25 miles",
            SearchRadiusMiles = 25,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            ProfileInterests = interestIds
                .Select(id => new ProfileInterest { InterestId = id, ExperienceLevelId = 1 })
                .ToList()
        };

        db.Profiles.Add(profile);
        return profile;
    }

    [Fact]
    public async Task GetCandidatesAsync_WhenProfileDoesNotExist_ReturnsEmptyList()
    {
        await using var db = CreateContext();

        var service = new MatchingService(db);
        var candidates = await service.GetCandidatesAsync(profileId: 999);

        Assert.Empty(candidates);
    }

    [Fact]
    public async Task GetCandidatesAsync_ExcludesTheRequestingProfileItself()
    {
        await using var db = CreateContext();
        var user = AddUser(db, "solo@example.com");
        var profile = AddProfile(db, user, "Solo", "GA", 1);
        await db.SaveChangesAsync();

        var service = new MatchingService(db);
        var candidates = await service.GetCandidatesAsync(profile.Id);

        Assert.Empty(candidates);
    }

    [Fact]
    public async Task GetCandidatesAsync_ExcludesProfilesWithNoSharedInterests()
    {
        await using var db = CreateContext();
        var requester = AddProfile(db, AddUser(db, "a@example.com"), "A", "GA", 1); // Hiking
        AddProfile(db, AddUser(db, "b@example.com"), "B", "GA", 2);                 // Kayaking only
        await db.SaveChangesAsync();

        var service = new MatchingService(db);
        var candidates = await service.GetCandidatesAsync(requester.Id);

        Assert.Empty(candidates);
    }

    [Fact]
    public async Task GetCandidatesAsync_IncludesProfileWithExactlyOneSharedInterest()
    {
        await using var db = CreateContext();
        var requester = AddProfile(db, AddUser(db, "a@example.com"), "A", "GA", 1);
        var other = AddProfile(db, AddUser(db, "b@example.com"), "B", "GA", 1, 2);
        await db.SaveChangesAsync();

        var service = new MatchingService(db);
        var candidates = await service.GetCandidatesAsync(requester.Id);

        var candidate = Assert.Single(candidates);
        Assert.Equal(other.Id, candidate.ProfileId);
        Assert.Equal(1, candidate.SharedInterestCount);
    }

    [Fact]
    public async Task GetCandidatesAsync_ExcludesProfilesInADifferentState()
    {
        await using var db = CreateContext();
        var requester = AddProfile(db, AddUser(db, "a@example.com"), "A", "GA", 1);
        AddProfile(db, AddUser(db, "b@example.com"), "B", "TX", 1); // same interest, different state
        await db.SaveChangesAsync();

        var service = new MatchingService(db);
        var candidates = await service.GetCandidatesAsync(requester.Id);

        Assert.Empty(candidates);
    }

    [Fact]
    public async Task GetCandidatesAsync_OrdersCandidatesByMostSharedInterestsFirst()
    {
        await using var db = CreateContext();
        var requester = AddProfile(db, AddUser(db, "a@example.com"), "A", "GA", 1, 2, 3);
        var oneShared = AddProfile(db, AddUser(db, "b@example.com"), "OneShared", "GA", 1);
        var threeShared = AddProfile(db, AddUser(db, "c@example.com"), "ThreeShared", "GA", 1, 2, 3);
        var twoShared = AddProfile(db, AddUser(db, "d@example.com"), "TwoShared", "GA", 1, 2);
        await db.SaveChangesAsync();

        var service = new MatchingService(db);
        var candidates = await service.GetCandidatesAsync(requester.Id);

        Assert.Equal(
            [threeShared.Id, twoShared.Id, oneShared.Id],
            candidates.Select(c => c.ProfileId));
    }

    [Fact]
    public async Task GetCandidatesAsync_WhenNoOtherProfilesExist_ReturnsEmptyList()
    {
        await using var db = CreateContext();
        var requester = AddProfile(db, AddUser(db, "a@example.com"), "A", "GA", 1);
        await db.SaveChangesAsync();

        var service = new MatchingService(db);
        var candidates = await service.GetCandidatesAsync(requester.Id);

        Assert.Empty(candidates);
    }

    // Acceptance-level (BDD / A-TDD) tests, tied to US-11's acceptance criteria:
    // "the API returns a filtered list of candidate profiles" based on shared
    // interests and location.

    [Fact]
    public async Task Given_TwoProfilesShareAnInterestInTheSameState_When_CandidatesAreRequested_Then_TheyAppearAsCandidatesForEachOther()
    {
        // Given two profiles in Georgia who both like hiking
        await using var db = CreateContext();
        var alice = AddProfile(db, AddUser(db, "alice@example.com"), "Alice", "GA", 1);
        var bob = AddProfile(db, AddUser(db, "bob@example.com"), "Bob", "GA", 1);
        await db.SaveChangesAsync();
        var service = new MatchingService(db);

        // When each requests their match candidates
        var aliceCandidates = await service.GetCandidatesAsync(alice.Id);
        var bobCandidates = await service.GetCandidatesAsync(bob.Id);

        // Then they show up as candidates for each other
        Assert.Contains(aliceCandidates, c => c.ProfileId == bob.Id);
        Assert.Contains(bobCandidates, c => c.ProfileId == alice.Id);
    }

    [Fact]
    public async Task Given_ACandidateIsInADifferentState_When_CandidatesAreRequested_Then_TheyDoNotAppearAsACandidate()
    {
        // Given two profiles who share an interest but live in different states
        await using var db = CreateContext();
        var georgia = AddProfile(db, AddUser(db, "ga@example.com"), "GeorgiaUser", "GA", 1);
        AddProfile(db, AddUser(db, "tx@example.com"), "TexasUser", "TX", 1);
        await db.SaveChangesAsync();
        var service = new MatchingService(db);

        // When the Georgia user requests their match candidates
        var candidates = await service.GetCandidatesAsync(georgia.Id);

        // Then the out-of-state user does not appear, matching the "within your area" acceptance criteria
        Assert.Empty(candidates);
    }
}
