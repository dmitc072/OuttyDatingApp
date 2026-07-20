# Sprint 2 Planning

**Sprint Goal:** Deliver a working matching engine where users can set their search radius, swipe on filtered profiles, match with users who swipe back, and receive a match notification. All matching data persists in Azure SQL.

**Sprint Dates:** July 13 – July 19, 2026 (7 days)

**Scrum Master:** Duane Mitchell
**Product Owner:** Jazmin Johnson
**Developer:** Yamani Barnes

---

## Velocity Forecast — Yesterday's Weather

**Sprint 1 actual velocity: 0 story points**

### Yesterday's Weather Pattern

Yesterday's Weather is a Scrum forecasting pattern that says:
> *"The best predictor of how much you can do this sprint is how much you actually completed last sprint."*

Strictly applied, that would forecast **0 points** for Sprint 2 — Sprint 1 shipped no completed stories (see `docs/Sprint 1/sprint-review.md`). We're overriding that forecast and explaining why below, rather than pretending Sprint 1's number was something it wasn't.

### Calculation

| Sprint                 | Planned    | Completed | Notes                                                                    |
| ----------------------- | ---------- | --------- | -------------------------------------------------------------------------- |
| Sprint 1                | 16 pts     | 0 pts     | Environment setup, inconsistent team participation — no story shipped     |
| **Sprint 2 Forecast**   | **17 pts** | —         | Overridden above Sprint 1 actual (see rationale)                          |

**Rationale for overriding Yesterday's Weather:**
- Sprint 1's 0-point actual reflects one-time setup cost (MAUI workloads, Azure CLI, repo/project scaffolding) and a team-participation gap identified in the Sprint 1 retrospective — not the team's real per-sprint capacity.
- Sprint 2 retrospective action items (Week 0 environment check, confirmed team availability at planning, daily GitHub Projects updates) directly target the causes of the 0-point outcome.
- Committing to a 0-point forecast would not exercise or validate the process improvements the team just committed to — 17 pts (matching Sprint 1's original *planned*, not actual, load) was chosen instead so Sprint 2 is a real test of whether those fixes worked.
- If Sprint 2 also comes in well under forecast, that's the signal to trust Yesterday's Weather literally for Sprint 3.

### Stories Committed to Sprint 2

| ID | Story | Points |
|---|---|---|
| US-08 | Set Search Radius | 2 |
| US-09 | Set Availability | 2 |
| US-10 | Swipe Left/Right UI | 5 |
| US-11 | Matching Algorithm | 5 |
| US-12 | Match Notification | 3 |
| **Total** | | **17 pts** |

---

## Sprint Backlog — Stories Decomposed into Tasks

### US-08 — Set Search Radius (2 pts)

| Task | Owner | Estimate | Status |
|---|---|---|---|
| Add search radius slider (5–100 miles) to PreferencesPage.xaml | Yamani | 1h | To Do |
| Save radius preference to Azure SQL via PreferencesService | Jazmin | 1h | To Do |
| Default radius to 25 miles if not set | Jazmin | 0.5h | To Do |
| Write unit tests for radius save and default logic | Duane | 1h | To Do |

### US-09 — Set Availability (2 pts)

| Task | Owner | Estimate | Status |
|---|---|---|---|
| Add availability selector (Weekdays / Weekends / Flexible) to PreferencesPage | Yamani | 1h | To Do |
| Save availability to user profile in Azure SQL | Jazmin | 1h | To Do |
| Display availability on profile card in swipe deck | Yamani | 0.5h | To Do |
| Write unit tests for availability save | Duane | 0.5h | To Do |

### US-10 — Swipe Left/Right UI (5 pts)

| Task | Owner | Estimate | Status |
|---|---|---|---|
| Design SwipePage.xaml in Figma (profile card layout) | Duane | 2h | To Do |
| Build SwipePage.xaml with profile card component in MAUI | Yamani | 3h | To Do |
| Implement PanGestureRecognizer for swipe left/right detection | Yamani | 2h | To Do |
| Animate card off screen on swipe (translate + fade) | Yamani | 1.5h | To Do |
| Record swipe result (like/pass) to Azure SQL via MatchService | Jazmin | 1.5h | To Do |
| Write unit tests for swipe recording logic | Duane | 1h | To Do |

> Design reference: [Discover screen](../design.md#discover)

### US-11 — Matching Algorithm (5 pts)

| Task | Owner | Estimate | Status |
|---|---|---|---|
| Design MatchingService — filter by shared activity type AND distance | Jazmin | 2h | To Do |
| Implement MatchingService.GetCandidates() in Outty.Core | Jazmin | 3h | To Do |
| Add /api/matches/candidates endpoint to Outty.Api | Jazmin | 1.5h | To Do |
| Connect SwipePage to candidates endpoint — load filtered deck | Yamani | 1.5h | To Do |
| Handle empty deck state (no more candidates) | Yamani | 0.5h | To Do |
| Write unit tests for MatchingService filter logic | Duane | 2h | To Do |
| BDD test: Given two users share Hiking preference and are within range, When they are loaded, Then they appear in each other's candidate deck | Duane | 1.5h | To Do |

### US-12 — Match Notification (3 pts)

| Task | Owner | Estimate | Status |
|---|---|---|---|
| Detect mutual like in MatchService.RecordSwipe() | Jazmin | 1h | To Do |
| Create Match record in Azure SQL when mutual like detected | Jazmin | 1h | To Do |
| Show in-app match alert on SwipePage when match occurs | Yamani | 1.5h | To Do |
| Add matched user to Matches list page | Yamani | 1h | To Do |
| Write unit tests for mutual like detection | Duane | 1h | To Do |

> Design reference: [Partners screen](../design.md#partners)

---

## Kanban Board

**GitHub Projects Board URL:** https://github.com/users/dmitc072/projects/3

> All tasks above should be added as cards to the board under the Sprint 2 milestone.
> Columns: To Do | In Progress | In Review | Done

---

## Definition of Done (Sprint 2)

A story is done when:
- [ ] Feature works end-to-end on the Android emulator
- [ ] All data persists in Azure SQL
- [ ] Unit tests written and passing (dotnet test shows green)
- [ ] CI pipeline runs and passes on GitHub Actions
- [ ] Code committed and pushed to the team's GitHub repository
- [ ] No critical bugs on the happy path
- [ ] Demoed to at least one other team member
