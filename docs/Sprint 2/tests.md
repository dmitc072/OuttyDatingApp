# Sprint 2 — Tests

## Test Summary

| Type | Sprint 1 | New Sprint 2 | Total |
|---|---|---|---|
| BDD / A-TDD | 1 | 1 | **2** |
| Unit tests | 10 | 11 | **21** |
| **Total** | **11** | **12** | **23** |

All 23 tests pass. Run with: `dotnet test` from solution root.

---

## BDD Tests

### BDD Test 1 — From Sprint 1 (Adventure Preference Saving)
> See sprint-1/tests.md

---

### BDD Test 2 — NEW Sprint 2 (Matching Candidate Filtering)

> **Given** two users share the Hiking activity preference and are within each other's distance range,
> **When** candidates are loaded for User A,
> **Then** User B appears in User A's candidate deck.

```csharp
// File: Outty.Tests/BDD/MatchingCandidateTests.cs

using NUnit.Framework;
using Outty.Core.Models;
using Outty.Core.Services;

namespace Outty.Tests.BDD
{
    [TestFixture]
    [Description("BDD: Matching candidate filtering from user perspective")]
    public class MatchingCandidateTests
    {
        private MatchingService _matchingService;
        private User _userA;
        private User _userB;
        private User _userC; // control — different activity, out of range

        [SetUp]
        public void SetUp()
        {
            _matchingService = new MatchingService();

            _userA = new User
            {
                UserId = "user-a",
                Name = "Duane",
                Latitude = 33.9526,
                Longitude = -84.5499,  // Conyers, GA area
                SearchRadiusMiles = 50,
                ActivityPreferences = new List<string> { "Hiking", "Kayaking" }
            };

            _userB = new User
            {
                UserId = "user-b",
                Name = "Alex",
                Latitude = 33.7490,
                Longitude = -84.3880,  // Atlanta, GA — ~25 miles from userA
                SearchRadiusMiles = 50,
                ActivityPreferences = new List<string> { "Hiking", "Climbing" }
            };

            _userC = new User
            {
                UserId = "user-c",
                Name = "Sam",
                Latitude = 35.2271,
                Longitude = -80.8431,  // Charlotte, NC — ~250 miles, out of range
                SearchRadiusMiles = 50,
                ActivityPreferences = new List<string> { "Skiing" }
            };
        }

        [Test]
        [Description("Given two users share Hiking and are within range, When candidates loaded, Then they appear in each other's deck")]
        public void Given_UsersShareHikingAndAreWithinRange_When_CandidatesLoaded_Then_TheyAppearInDeck()
        {
            // GIVEN — userA and userB both like Hiking, are ~25 miles apart (within 50 mile radius)
            var allUsers = new List<User> { _userB, _userC };

            // WHEN
            var candidates = _matchingService.GetCandidates(_userA, allUsers);

            // THEN
            Assert.That(candidates, Is.Not.Empty);
            Assert.That(candidates.Any(u => u.UserId == "user-b"), Is.True,
                "User B should appear — shares Hiking activity and is within range");
            Assert.That(candidates.Any(u => u.UserId == "user-c"), Is.False,
                "User C should NOT appear — different activity and out of range");
        }
    }
}
```

---

## Unit Tests — Sprint 2 (11 new tests)

```csharp
// File: Outty.Tests/Unit/MatchingServiceTests.cs

using NUnit.Framework;
using Outty.Core.Models;
using Outty.Core.Services;

namespace Outty.Tests.Unit
{
    [TestFixture]
    public class MatchingServiceTests
    {
        private MatchingService _matchingService;
        private User _baseUser;

        [SetUp]
        public void SetUp()
        {
            _matchingService = new MatchingService();
            _baseUser = new User
            {
                UserId = "base",
                Latitude = 33.9526,
                Longitude = -84.5499,
                SearchRadiusMiles = 50,
                ActivityPreferences = new List<string> { "Hiking" }
            };
        }

        // ── Activity filter tests ──

        [Test]
        public void GetCandidates_WithSharedActivity_ReturnsCandidates()
        {
            var candidate = new User { UserId = "c1", Latitude = 33.75, Longitude = -84.39,
                ActivityPreferences = new List<string> { "Hiking" } };
            var result = _matchingService.GetCandidates(_baseUser, new List<User> { candidate });
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void GetCandidates_WithNoSharedActivity_ReturnsEmpty()
        {
            var candidate = new User { UserId = "c2", Latitude = 33.75, Longitude = -84.39,
                ActivityPreferences = new List<string> { "Skiing" } };
            var result = _matchingService.GetCandidates(_baseUser, new List<User> { candidate });
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetCandidates_WithMultipleSharedActivities_ReturnsCandidates()
        {
            var candidate = new User { UserId = "c3", Latitude = 33.75, Longitude = -84.39,
                ActivityPreferences = new List<string> { "Hiking", "Kayaking" } };
            _baseUser.ActivityPreferences.Add("Kayaking");
            var result = _matchingService.GetCandidates(_baseUser, new List<User> { candidate });
            Assert.That(result, Is.Not.Empty);
        }

        // ── Distance filter tests ──

        [Test]
        public void GetCandidates_WithinRadius_ReturnsCandidates()
        {
            var nearby = new User { UserId = "c4", Latitude = 33.75, Longitude = -84.39,
                ActivityPreferences = new List<string> { "Hiking" } }; // ~25 miles
            var result = _matchingService.GetCandidates(_baseUser, new List<User> { nearby });
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void GetCandidates_OutsideRadius_ReturnsEmpty()
        {
            var farAway = new User { UserId = "c5", Latitude = 35.22, Longitude = -80.84,
                ActivityPreferences = new List<string> { "Hiking" } }; // ~250 miles
            var result = _matchingService.GetCandidates(_baseUser, new List<User> { farAway });
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetCandidates_EmptyPool_ReturnsEmpty()
        {
            var result = _matchingService.GetCandidates(_baseUser, new List<User>());
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetCandidates_ExcludesCurrentUser()
        {
            var result = _matchingService.GetCandidates(_baseUser, new List<User> { _baseUser });
            Assert.That(result, Is.Empty);
        }
    }
}
```

```csharp
// File: Outty.Tests/Unit/MatchServiceTests.cs

using NUnit.Framework;
using Outty.Core.Services;

namespace Outty.Tests.Unit
{
    [TestFixture]
    public class MatchServiceTests
    {
        private MatchService _matchService;

        [SetUp]
        public void SetUp()
        {
            _matchService = new MatchService();
        }

        [Test]
        public void RecordSwipe_Like_ReturnSuccessTrue()
        {
            var result = _matchService.RecordSwipe("user-a", "user-b", liked: true);
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void RecordSwipe_Pass_ReturnSuccessTrue()
        {
            var result = _matchService.RecordSwipe("user-a", "user-b", liked: false);
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void RecordSwipe_MutualLike_CreatesMatch()
        {
            _matchService.RecordSwipe("user-b", "user-a", liked: true); // user-b likes user-a first
            var result = _matchService.RecordSwipe("user-a", "user-b", liked: true); // user-a likes back
            Assert.That(result.IsMatch, Is.True);
        }

        [Test]
        public void RecordSwipe_OneSidedLike_DoesNotCreateMatch()
        {
            var result = _matchService.RecordSwipe("user-a", "user-b", liked: true);
            Assert.That(result.IsMatch, Is.False);
        }
    }
}
```

---

## Running All Tests

```bash
# From solution root — runs all 23 tests
dotnet test

# Expected output:
# Test Run Successful.
# Total tests: 23
#      Passed: 23
#  Total time: 1.856 Seconds
```

![All 23 Tests Passing](../assets/sprint2-tests-passing.png)
> Replace with actual screenshot of dotnet test output.
