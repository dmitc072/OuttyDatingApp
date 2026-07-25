# Sprint 3 Planning

**Sprint Goal:** Deliver the matching engine's backend — users can set a search radius preference, and the API returns a filtered list of candidate profiles they can be shown to swipe on. This unblocks Sprint 4's swipe UI, which needs real candidate data to build against instead of placeholders.

**Sprint Dates:** July 20 – July 26, 2026 (7 days)

**Scrum Master:** Duane Mitchell
**Product Owner:** Jazmin Johnson
**Developer:** Yamani Barnes

---

## Velocity Forecast — Yesterday's Weather

**Sprint 2 actual velocity:**

| Measure | Points | What it counts |
|---|---|---|
| Committed-and-finished (strict) | 3 pts | US-12 only — the one story fully completed that was actually committed to Sprint 2 |
| Total shipped (all work) | 16 pts | 3 pts (US-12) + 13 pts Sprint 1 carryover (US-01–US-04) finished this week |

### Applying Yesterday's Weather honestly

Strictly applied, Yesterday's Weather says: forecast what you actually finished last time. That's **3 points** if you count only freshly-committed work, or **16** if you count everything shipped including the Sprint 1 backlog cleanup. Neither number, used on its own, is the right forecast for Sprint 3:

- **16 is inflated.** Most of it (13 pts) was one-time carryover cleanup, not repeatable per-sprint throughput. Using it again would repeat exactly the mistake flagged in Sprint 2's feedback — committing to a number the team hasn't actually demonstrated it can deliver in a single sprint.
- **3 is overly conservative for this specific sprint**, for two reasons: (1) US-11 and US-10 were already marked "In Progress" at the end of Sprint 2 — this isn't cold-start work, some of it is underway; (2) **only one sprint remains after this one** to complete the app (Part 5/Sprint 4), so treating 3 points as a hard ceiling would make it structurally impossible to finish the matching-engine stories at all before the course ends.

### Sprint 3 Forecast: 7 points

Rather than re-committing to all 14 points of Sprint 2's unfinished work (repeating the overcommitment), Sprint 3 commits to **half of it** — the two stories that form the matching engine's backend, sequenced to unblock Sprint 4's UI work:

| ID | Story | Points |
|---|---|---|
| US-11 | Matching Algorithm | 5 |
| US-08 | Set Search Radius | 2 |
| **Total** | | **7 pts** |

**US-09 (Set Availability, 2 pts) and US-10 (Swipe Left/Right UI, 5 pts) are deliberately deferred to Sprint 4.** This isn't scope-cutting for its own sake — US-10 (the swipe UI) needs a real candidates endpoint to swipe *through*. Building it before US-11 exists means building against fake data and rewiring it later. Doing the matching backend first is the logical dependency order, not just a smaller number.

**Rationale for the 7-point number specifically:** it sits between the two Yesterday's Weather extremes (3 and 16), matches exactly two stories the team can commit to *end-to-end* rather than spreading effort across four again, and splits the remaining 14 points of matching-engine work evenly across the two sprints left (7 this sprint, 7 next) instead of front- or back-loading it.

---

## Sprint Backlog — Stories Decomposed into Tasks

### US-08 — Set Search Radius (2 pts) — Backend Done, UI Pending

| Task | Owner | Estimate | Status |
|---|---|---|---|
| Add `SearchRadiusMiles` validation logic (5–100 miles, default 25) as a testable unit in `Outty.Shared` | Duane | 1h | Done |
| Add `dbo.Profiles.SearchRadiusMiles` column via `db/schema.sql`, re-scaffold EF models | Duane | 1h | Done |
| Add search radius slider (5–100 miles) to `PreferencesPage.xaml`, defaulting to 25 | Yamani | 1h | To Do |
| Wire the slider to save via the API (`PUT /profiles/{id}/search-radius` now exists) | Jazmin | 1h | To Do |
| Unit tests for radius validation (below min, above max, default, boundaries) | Duane | 1h | Done |

### US-11 — Matching Algorithm (5 pts) — Backend Done, UI Pending

| Task | Owner | Estimate | Status |
|---|---|---|---|
| Implement `MatchingService.GetCandidates()` — filters by shared interests + state, excludes self | Duane | 3h | Done |
| Expose `GET /matches/candidates/{profileId}` endpoint in `Outty.Api` | Duane | 1h | Done |
| Unit tests for `MatchingService` filter logic (shared/no shared interests, self-exclusion, empty results, ordering) | Duane | 2h | Done |
| BDD/A-TDD tests: acceptance criteria for shared-interest matching and out-of-state exclusion | Duane | 1h | Done |
| Connect `SwipePage` scaffold to the candidates endpoint (stub UI acceptable — full swipe UI is Sprint 4/US-10) | Yamani | 2h | To Do |

> Design reference: [Discover screen](../design.md#discover)

---

## Kanban Board

**GitHub Projects Board URL:** https://github.com/users/dmitc072/projects/3

---

## Definition of Done (Sprint 3)

A story is done when:

- [ ] Feature logic implemented and covered by passing unit tests
- [ ] `MatchingService`/API logic testable and tested without requiring a live database connection in CI
- [ ] All data persists in Azure SQL where applicable
- [ ] CI pipeline runs and passes on GitHub Actions
- [ ] Code committed and pushed to the team's GitHub repository
- [ ] No critical bugs on the happy path
- [ ] Demoed to at least one other team member
