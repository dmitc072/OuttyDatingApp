# Sprint 2 Planning

**Sprint Goal:** Deliver a working matching engine where users can set their search radius, swipe on filtered profiles, match with users who swipe back, and receive a match notification. All matching data persists in Azure SQL.

**Sprint Dates:** [Start Date] — [End Date] (2 weeks)

**Scrum Master:** Duane Mitchell
**Product Owner:** [Member 2 Name]
**Developer:** [Member 3 Name]

---

## Velocity Forecast — Yesterday's Weather

**Forecast: [X] story points**

> Replace [X] with your Sprint 1 actual completed points.

### Yesterday's Weather Pattern

Yesterday's Weather is a Scrum forecasting pattern that says:
> *"The best predictor of how much you can do this sprint is how much you actually completed last sprint."*

Rather than guessing or estimating from scratch, we use our Sprint 1 actual velocity as our Sprint 2 forecast.

### Calculation

| Sprint | Planned | Completed | Notes |
|---|---|---|---|
| Sprint 1 | 16 pts | [X] pts | First sprint — environment setup cost us ~2 days |
| **Sprint 2 Forecast** | **[X] pts** | — | Yesterday's Weather: equal to Sprint 1 actual |

**Rationale:**
- Sprint 1 actual velocity = **[X] story points**
- Sprint 2 forecast = **[X] story points** (same, per Yesterday's Weather)
- We are not adjusting the forecast upward even though we expect to be faster in Sprint 2 now that the environment is set up — Yesterday's Weather uses the number as-is
- If we complete all forecasted stories early, we will pull the next highest-priority item from the backlog

### Stories Committed to Sprint 2

| ID | Story | Points |
|---|---|---|
| US-08 | Set Search Radius | 2 |
| US-09 | Set Availability | 2 |
| US-10 | Swipe Left/Right UI | 5 |
| US-11 | Matching Algorithm | 5 |
| US-12 | Match Notification | 3 |
| **Total** | | **17 pts** |

> If Sprint 1 actual was less than 17, remove the lowest-priority story (US-09) and adjust the total. If it was more, consider pulling US-05 (Instagram) from the backlog.

---

## Sprint Backlog — Stories Decomposed into Tasks

### US-08 — Set Search Radius (2 pts)

| Task | Owner | Estimate | Status |
|---|---|---|---|
| Add search radius slider (5–100 miles) to PreferencesPage.xaml | [Member 3] | 1h | To Do |
| Save radius preference to Azure SQL via PreferencesService | [Member 2] | 1h | To Do |
| Default radius to 25 miles if not set | [Member 2] | 0.5h | To Do |
| Write unit tests for radius save and default logic | Duane | 1h | To Do |

### US-09 — Set Availability (2 pts)

| Task | Owner | Estimate | Status |
|---|---|---|---|
| Add availability selector (Weekdays / Weekends / Flexible) to PreferencesPage | [Member 3] | 1h | To Do |
| Save availability to user profile in Azure SQL | [Member 2] | 1h | To Do |
| Display availability on profile card in swipe deck | [Member 3] | 0.5h | To Do |
| Write unit tests for availability save | Duane | 0.5h | To Do |

### US-10 — Swipe Left/Right UI (5 pts)

| Task | Owner | Estimate | Status |
|---|---|---|---|
| Design SwipePage.xaml in Figma (profile card layout) | Duane | 2h | To Do |
| Build SwipePage.xaml with profile card component in MAUI | [Member 3] | 3h | To Do |
| Implement PanGestureRecognizer for swipe left/right detection | [Member 3] | 2h | To Do |
| Animate card off screen on swipe (translate + fade) | [Member 3] | 1.5h | To Do |
| Record swipe result (like/pass) to Azure SQL via MatchService | [Member 2] | 1.5h | To Do |
| Write unit tests for swipe recording logic | Duane | 1h | To Do |

### US-11 — Matching Algorithm (5 pts)

| Task | Owner | Estimate | Status |
|---|---|---|---|
| Design MatchingService — filter by shared activity type AND distance | [Member 2] | 2h | To Do |
| Implement MatchingService.GetCandidates() in Outty.Core | [Member 2] | 3h | To Do |
| Add /api/matches/candidates endpoint to Outty.Api | [Member 2] | 1.5h | To Do |
| Connect SwipePage to candidates endpoint — load filtered deck | [Member 3] | 1.5h | To Do |
| Handle empty deck state (no more candidates) | [Member 3] | 0.5h | To Do |
| Write unit tests for MatchingService filter logic | Duane | 2h | To Do |
| BDD test: Given two users share Hiking preference and are within range, When they are loaded, Then they appear in each other's candidate deck | Duane | 1.5h | To Do |

### US-12 — Match Notification (3 pts)

| Task | Owner | Estimate | Status |
|---|---|---|---|
| Detect mutual like in MatchService.RecordSwipe() | [Member 2] | 1h | To Do |
| Create Match record in Azure SQL when mutual like detected | [Member 2] | 1h | To Do |
| Show in-app match alert on SwipePage when match occurs | [Member 3] | 1.5h | To Do |
| Add matched user to Matches list page | [Member 3] | 1h | To Do |
| Write unit tests for mutual like detection | Duane | 1h | To Do |

---

## Kanban Board

**GitHub Projects Board URL:** https://github.com/[your-username]/Outty/projects/1

> All tasks above should be added as cards to the board under the Sprint 2 milestone.
> Columns: To Do | In Progress | In Review | Done

![Kanban Board](../assets/sprint2-kanban.png)
> Replace with actual screenshot once tasks are on the board.

---

## Definition of Done (Sprint 2)

A story is done when:
- [ ] Feature works end-to-end on iOS simulator
- [ ] All data persists in Azure SQL
- [ ] Unit tests written and passing (dotnet test shows green)
- [ ] CI pipeline runs and passes on GitHub Actions
- [ ] Code pushed to `/sprint-2/` folder on GitHub
- [ ] No critical bugs on the happy path
- [ ] Demoed to at least one other team member
