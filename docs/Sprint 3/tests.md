# Sprint 3 — Tests

## Test Projects

| Project | Covers | Run locally |
|---|---|---|
| [tests/Outty.Shared.Tests](../../tests/Outty.Shared.Tests) | `AgeCalculator`, `SearchRadiusValidator` | `dotnet test tests/Outty.Shared.Tests/Outty.Shared.Tests.csproj` |
| [tests/Outty.Api.Tests](../../tests/Outty.Api.Tests) | `MatchingService` (EF Core InMemory — no live database needed) | `dotnet test tests/Outty.Api.Tests/Outty.Api.Tests.csproj` |

**Run in CI:** every push/PR to `main`/`dev` — both test projects run as separate steps in `.github/workflows/main.yml`.

---

## What's New This Sprint

**`SearchRadiusValidator`** ([src/Outty.Shared/Utilities/SearchRadiusValidator.cs](../../src/Outty.Shared/Utilities/SearchRadiusValidator.cs)) — validates and normalizes the US-08 search radius preference (5–100 miles, defaults to 25). Backs the `PUT /profiles/{id}/search-radius` endpoint in `Outty.Api`.

**`MatchingService`** ([src/Outty.Api/Services/MatchingService.cs](../../src/Outty.Api/Services/MatchingService.cs)) — the US-11 matching algorithm. Given a profile, returns other profiles in the same state who share at least one interest, ordered by most shared interests first. Backs the `GET /matches/candidates/{profileId}` endpoint. Tested against an EF Core in-memory database rather than live Azure SQL — the point of testing is to verify the filtering/ordering logic itself, not the database connection, and CI runners aren't in the SQL server's firewall allow-list anyway.

---

## Test Counts

| Type | Sprint 2 total | New this sprint | Sprint 3 total |
|---|---|---|---|
| Unit tests | 20 | 19 | 39 |
| BDD / A-TDD tests | 2 | 3 | 5 |
| **Total** | **22** | **22** | **44** |

All 44 tests pass (`dotnet test`, verified locally and in CI). Rubric requires 30+ total / 10+ new / 3+ BDD total / 1+ new BDD — all exceeded.

### Where the new tests came from

- `SearchRadiusValidatorTests` — 15 unit tests (`IsValid` boundaries, `Normalize` clamping) + 1 BDD/A-TDD test (default-radius acceptance criteria)
- `MatchingServiceTests` — 7 unit tests (missing profile, self-exclusion, no-shared-interest exclusion, single shared interest, different-state exclusion, ordering by shared-interest count, empty-database case) + 2 BDD/A-TDD tests (two profiles matching across a shared interest; a same-interest profile in a different state correctly excluded)

---

## Live Verification (not just unit tests)

Beyond the test suite, the actual API was run locally against the real Azure SQL database and hit directly:

```
GET /states                          → 200, 51 rows (unchanged from Sprint 2)
GET /matches/candidates/1            → 200, [] (no profiles exist yet — correct)
PUT /profiles/1/search-radius        → 404 (profile 1 doesn't exist — correct)
```

## Next Sprint

Sprint 4 is UI-focused (US-09 Set Availability, US-10 Swipe Left/Right UI) — `SwipePage` needs to call `GET /matches/candidates/{profileId}`, which now exists and is tested. Once real profile data exists, a manual/exploratory pass against the live database (not just InMemory) is worth doing to confirm the EF Core query translates correctly to real SQL.
