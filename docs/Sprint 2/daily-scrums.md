# Sprint 2 — Daily Scrum Logs

Daily scrums held asynchronously via Discord every morning at 9:00 AM. Each member posts their three items. Scrum Master (Duane) flags impediments and follows up on blockers same day.

---

## Daily Scrum — Day 1 | [Date]

### Duane Mitchell (Scrum Master)

**What did I do in the last 24 hours?**
Conducted Sprint 1 retrospective, updated sprint-1/retrospective.md in the repo, and set up Sprint 2 planning.md with all stories decomposed into tasks. Moved Sprint 2 cards to the GitHub Projects board.

**What will I do in the next 24 hours?**
Begin Figma design for SwipePage profile card layout. Set up GitHub Actions CI workflow file so every push triggers a build and test run.

**Any impediments?**
None. Sprint 1 retrospective action items assigned — team confirmed availability for two planned pairing sessions this sprint.

---

### [Member 2 Name] (Product Owner)

**What did I do in the last 24 hours?**
Reviewed and accepted all Sprint 1 completed stories in the backlog. Updated backlog priorities for Sprint 2 — confirmed US-08 through US-12 in scope. Began designing MatchingService schema in Outty.Core.

**What will I do in the next 24 hours?**
Implement MatchingService.GetCandidates() with activity type and distance filters. Add Azure SQL migration for Match table (UserId, CandidateId, Liked, Timestamp).

**Any impediments?**
None.

---

### [Member 3 Name] (Developer)

**What did I do in the last 24 hours?**
Pulled Sprint 2 branch, reviewed all task assignments. Reviewed MAUI PanGestureRecognizer docs to prepare for swipe implementation.

**What will I do in the next 24 hours?**
Add search radius slider to PreferencesPage.xaml (US-08). Begin SwipePage.xaml layout following Duane's Figma design once it is shared.

**Any impediments?**
Need Figma link from Duane before building SwipePage. Expected today.

---

## Daily Scrum — Day 4 | [Date]

### Duane Mitchell (Scrum Master)

**What did I do in the last 24 hours?**
Completed SwipePage Figma design and shared link in Discord. Set up GitHub Actions CI — workflow file committed, pipeline running on every push to main. Wrote 6 new unit tests for MatchingService filter logic.

**What will I do in the next 24 hours?**
Write BDD test for US-11 candidate filtering. Pair with [Member 3] on PanGestureRecognizer swipe animation (Planned Pairing Session 1).

**Any impediments?**
None. CI pipeline is green.

---

### [Member 2 Name] (Product Owner)

**What did I do in the last 24 hours?**
Completed MatchingService.GetCandidates() with activity + distance filtering. Added /api/matches/candidates endpoint. Tested with Postman — returns correct filtered candidates.

**What will I do in the next 24 hours?**
Implement MatchService.RecordSwipe() to save like/pass to Azure SQL. Add mutual like detection logic — when both users like each other, create a Match record.

**Any impediments?**
None.

---

### [Member 3 Name] (Developer)

**What did I do in the last 24 hours?**
Built SwipePage.xaml profile card layout matching Figma design. Implemented PanGestureRecognizer — card tracks finger on screen. Left/right threshold detection working.

**What will I do in the next 24 hours?**
Add swipe animation (translate + fade card off screen). Connect SwipePage to /api/matches/candidates to load real candidate data.

**Any impediments?**
None.

---

## Daily Scrum — Day 8 | [Date]

### Duane Mitchell (Scrum Master)

**What did I do in the last 24 hours?**
Completed BDD test for US-11. Total test count now at 23 (2 BDD + 21 unit). All passing in CI. Conducted Planned Pairing Session 2 with [Member 2] on mutual like detection logic.

**What will I do in the next 24 hours?**
Write remaining unit tests to ensure 20+ total. Begin sprint review prep — compile demo script and screenshots.

**Any impediments?**
None.

---

### [Member 2 Name] (Product Owner)

**What did I do in the last 24 hours?**
Completed MatchService.RecordSwipe() with mutual like detection. Match record created in Azure SQL on mutual like. Verified end-to-end with Postman — two test users liking each other creates a Match correctly.

**What will I do in the next 24 hours?**
Add matched user to Matches list page in MAUI. Wire up match notification alert on SwipePage when mutual like is detected.

**Any impediments?**
None.

---

### [Member 3 Name] (Developer)

**What did I do in the last 24 hours?**
Swipe animation complete — card translates and fades off screen on swipe. Connected SwipePage to candidates API — real profiles load in the deck. Empty deck state shows "No more adventurers nearby" message.

**What will I do in the next 24 hours?**
Add in-app match alert popup on SwipePage when mutual like occurs. Add matched user to Matches list. Test full swipe → match → notification flow end-to-end.

**Any impediments?**
None.

---

## Impediment Log

| Date | Impediment | Owner | Status | Resolution |
|---|---|---|---|---|
| Day 1 | [Member 3] waiting for Figma SwipePage design | Duane | Resolved | Figma link shared Day 1 afternoon |

---

## Pair Programming Evidence

**Planned Pairing Session 1 — Day 4**
Duane (navigator) + [Member 3] (driver) — PanGestureRecognizer swipe animation implementation. 90 minutes via Discord screen share.

**Planned Pairing Session 2 — Day 7**
Duane (driver) + [Member 2] (navigator) — mutual like detection and Match record creation. 60 minutes via Discord screen share.

![Pairing Session 1 Screenshot](../assets/sprint2-pairing-1.png)
![Pairing Session 2 Screenshot](../assets/sprint2-pairing-2.png)

> Replace with actual screenshots of Discord screen share sessions.

**Co-authored commit evidence:**
```
git commit -m "Implement PanGestureRecognizer swipe animation on SwipePage

Co-authored-by: [Member3Name] <member3@email.com>"
```
