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

> _"The best predictor of how much you can do this sprint is how much you actually completed last sprint."_

Strictly applied, that would forecast **0 points** for Sprint 2 — Sprint 1 shipped no completed stories (see `docs/Sprint 1/sprint-review.md`). We're overriding that forecast and explaining why below, rather than pretending Sprint 1's number was something it wasn't.

### Calculation

| Sprint                      | Planned    | Completed  | Notes                                                                                            |
| --------------------------- | ---------- | ---------- | ------------------------------------------------------------------------------------------------ |
| Sprint 1                    | 16 pts     | 0 pts      | Environment setup, inconsistent team participation — no story shipped                            |
| **Sprint 2 Forecast**       | **17 pts** | **3 pts**  | Only US-12 (committed) finished; see below for total velocity including carryover                |
| Sprint 2 (actual, all work) | —          | **16 pts** | 3 pts committed (US-12) + 13 pts Sprint 1 carryover (US-01–US-04) — see `sprint-review-retro.md` |

> This table was written at Sprint 2 planning time and forecast 17 pts. The "Completed" column above was filled in after the sprint ended to close the loop — actual committed-story velocity (3 pts) is what Sprint 3's Yesterday's Weather forecast should be judged against if you want to compare like-for-like with a forecast; the 16-pt total (see `sprint-review-retro.md`) is what the team's real throughput was, carryover included.

**Rationale for overriding Yesterday's Weather:**

- Sprint 1's 0-point actual reflects one-time setup cost (MAUI workloads, Azure CLI, repo/project scaffolding) and a team-participation gap identified in the Sprint 1 retrospective, not the team's real per-sprint capacity.
- Sprint 2 retrospective action items (Week 0 environment check, confirmed team availability at planning, daily GitHub Projects updates) directly target the causes of the 0-point outcome.
- Committing to a 0-point forecast would not exercise or validate the process improvements the team just committed to — 17 pts (matching Sprint 1's original _planned_, not actual, load) was chosen instead so Sprint 2 is a real test of whether those fixes worked.
- If Sprint 2 also comes in well under forecast, that's the signal to trust Yesterday's Weather literally for Sprint 3.

### Stories Committed to Sprint 2

| ID        | Story               | Points     | End-of-Sprint Status |
| --------- | ------------------- | ---------- | -------------------- |
| US-08     | Set Search Radius   | 2          | Not Started          |
| US-09     | Set Availability    | 2          | Not Started          |
| US-10     | Swipe Left/Right UI | 5          | In Progress          |
| US-11     | Matching Algorithm  | 5          | In Progress          |
| US-12     | Set up Database     | 3          | ✅ Done              |
| **Total** |                     | **17 pts** | **3 pts done**       |

> See `sprint-review-retro.md` for the full outcome, including Sprint 1 carryover stories (US-01–US-04) also finished this week.

---

## Sprint Backlog — Stories Decomposed into Tasks

### US-08 — Set Search Radius (2 pts) — Not Started

| Task                                                                                          | Owner  | Estimate | Status |
| --------------------------------------------------------------------------------------------- | ------ | -------- | ------ |
| Add search radius slider (5–100 miles) to PreferencesPage.xaml                                | Yamani | 1h       | To Do  |
| Save radius preference to Azure SQL via PreferencesService, defaulting to 25 miles if not set | Jazmin | 1.5h     | To Do  |
| Write unit tests for radius save and default logic                                            | Duane  | 1h       | To Do  |

### US-09 — Set Availability (2 pts) — Not Started

| Task                                                                             | Owner  | Estimate | Status |
| -------------------------------------------------------------------------------- | ------ | -------- | ------ |
| Add availability selector to PreferencesPage; show it on swipe deck profile card | Yamani | 1.5h     | To Do  |
| Save availability to user profile in Azure SQL                                   | Jazmin | 1h       | To Do  |
| Write unit tests for availability save                                           | Duane  | 0.5h     | To Do  |

### US-10 — Swipe Left/Right UI (5 pts) — In Progress

| Task                                                                              | Owner  | Estimate | Status |
| --------------------------------------------------------------------------------- | ------ | -------- | ------ |
| Design SwipePage.xaml in Figma (profile card layout)                              | Duane  | 2h       | To Do  |
| Build SwipePage.xaml: profile card, swipe gesture detection, off-screen animation | Jazmin | 6.5h     | To Do  |
| Record swipe result (like/pass) to Azure SQL via MatchService                     | Jazmin | 1.5h     | To Do  |
| Write unit tests for swipe recording logic                                        | Duane  | 1h       | To Do  |

> Design reference: [Discover screen](../design.md#discover)

### US-11 — Matching Algorithm (5 pts) — In Progress

| Task                                                                                    | Owner  | Estimate | Status |
| --------------------------------------------------------------------------------------- | ------ | -------- | ------ |
| Implement MatchingService.GetCandidates() (activity + distance filters), expose via API | Jazmin | 6.5h     | To Do  |
| Connect SwipePage to candidates endpoint and handle empty deck state                    | Yamani | 2h       | To Do  |
| Write unit + BDD tests for MatchingService filter logic                                 | Duane  | 3.5h     | To Do  |

### US-12 — Set up Database (3 pts) — ✅ Done

| Task                                                                         | Owner | Estimate | Status |
| ---------------------------------------------------------------------------- | ----- | -------- | ------ |
| Design Azure SQL schema for Match, Swipe, and Preference tables              | Duane | 2h       | Done   |
| Write and apply EF Core migrations; configure connection string in Outty.Api | Duane | 2.5h     | Done   |
| Write unit tests confirming DbContext resolves and migrations apply cleanly  | Duane | 1h       | Done   |

---

## Kanban Board

**GitHub Projects Board URL:** https://github.com/users/dmitc072/projects/3

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
