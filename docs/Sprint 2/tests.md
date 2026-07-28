# Sprint 2 — Tests

## Test Project

**Location:** [tests/Outty.Shared.Tests](../../tests/Outty.Shared.Tests)
**Framework:** xUnit
**Run locally:**

```bash
dotnet test tests/Outty.Shared.Tests/Outty.Shared.Tests.csproj
```

**Run in CI:** every push/PR to `main`/`dev` — see `.github/workflows/main.yml` and `ci-cd.md`.

---

## What's Under Test

`AgeCalculator` ([src/Outty.Shared/Utilities/AgeCalculator.cs](../../src/Outty.Shared/Utilities/AgeCalculator.cs)) — the "must be at least 18 years old" rule enforced on `CreateProfilePage` (US-02, Create Profile). This logic used to live as a private method directly inside `CreateProfilePage.xaml.cs`, which made it untestable without a full MAUI UI test harness. It was extracted into `Outty.Shared` this sprint specifically so it could be unit tested, and `CreateProfilePage` now calls the shared, tested version instead of its own copy.

---

## Test Counts (Sprint 2)

| Type | Count | New this sprint |
|---|---|---|
| Unit tests (`CalculateAge_ReturnsExpectedAge`, `IsAtLeast18_ReturnsExpectedEligibility`) | 20 | 20 |
| BDD / A-TDD tests (Given/When/Then, tied to the profile-creation acceptance criteria) | 2 | 2 |
| **Total** | **22** | **22** |

All 22 tests pass (`dotnet test`, verified locally and in CI).

Unit tests cover boundary conditions: exact-birthday-today, one day before/after turning 18, a newborn (age 0), a 100-year-old, birthdays that have/haven't occurred yet this calendar year, and leap-day (Feb 29) birthdates evaluated in non-leap years.

The 2 BDD/A-TDD tests are written in Given/When/Then form and assert against the actual product rule (a user can/cannot create a profile), not just the raw math:

```csharp
[Fact]
public void Given_UserTurnedEighteenToday_When_CheckingProfileEligibility_Then_TheyCanCreateAProfile()
{
    // Given a user whose 18th birthday is today
    var birthDate = new DateTime(2008, 7, 19);
    var today = new DateTime(2026, 7, 19);

    // When checking whether they're eligible to create a profile
    var isEligible = AgeCalculator.IsAtLeast18(birthDate, today);

    // Then they are allowed to proceed
    Assert.True(isEligible, "A user who turns 18 today should be allowed to create a profile.");
}
```

---

## Test-First Note

Writing these tests surfaced two incorrect assumptions in the *test data itself* (expected `true` for a 16- and 17-year-old, which should be `false`) — caught immediately by running `dotnet test` and fixed before commit. `AgeCalculator`'s actual logic was correct throughout; the mistake was in the test cases, and the test run is what caught it.

## Next Sprint

Sprint 3 should expand coverage into `Outty.Api` — once `MatchingService` (US-11) and preference-saving logic (US-08/US-09) exist, they should get unit tests the same way: pure, testable methods extracted where needed, exercised directly rather than through the UI or a live database.
