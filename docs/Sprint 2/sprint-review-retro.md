# Sprint 2 — Sprint Review

**Date:** [Date]
**Duration:** 30 minutes
**Attendees:** Duane Mitchell (SM), [Member 2] (PO), [Member 3] (Dev)

---

## Sprint Goal Review

**Sprint Goal:** Deliver a working matching engine where users can set their search radius, swipe on filtered profiles, match with users who swipe back, and receive a match notification.

**Goal Met:** ✅ Yes / ❌ No

---

## Stories Completed

| ID | Story | Points | Status |
|---|---|---|---|
| US-08 | Set Search Radius | 2 | ✅ Done |
| US-09 | Set Availability | 2 | ✅ Done |
| US-10 | Swipe Left/Right UI | 5 | ✅ Done |
| US-11 | Matching Algorithm | 5 | ✅ Done |
| US-12 | Match Notification | 3 | ✅ Done |
| **Total** | | **17 pts** | |

---

## Demo Notes

**US-10 + US-11 — Swipe + Matching:**
- Opened SwipePage — profile cards loaded from API (filtered by shared activity + distance)
- Swiped right on a candidate — card animated off screen, like recorded in Azure SQL
- Swiped left on another — card animated off screen, pass recorded

**US-12 — Match Notification:**
- Two test accounts mutually liked each other
- Match alert popup appeared on screen immediately
- Matched user appeared in Matches list

**CI Pipeline:**
- Showed GitHub Actions tab — green checkmarks on last 5 commits
- Demonstrated that pushing a failing test causes the CI to go red

---

## Working Software

**Link:** [Azure App Service URL or Loom screen recording link]

![Swipe Screen](../assets/sprint2-demo-swipe.png)
![Match Notification](../assets/sprint2-demo-match.png)

---

## Actual Velocity

**Planned:** 17 story points
**Completed:** [X] story points

> Used for Sprint 3 Yesterday's Weather forecast.

---

---

# Sprint 2 — Retrospective

**Date:** [Date]
**Duration:** 20 minutes
**Facilitator:** Duane Mitchell

---

## What Went Well (Continue)

- **CI saved us twice** — GitHub Actions caught a breaking test before it merged to main on Day 5 and Day 8. Without CI we would have discovered these bugs much later
- **Planned pairing sessions worked better than reactive ones** — scheduling them at planning meant they actually happened. Sprint 1 pairing felt rushed
- **Yesterday's Weather was accurate** — our Sprint 1 actual velocity was a reliable predictor. We finished on track without needing to descope

---

## What Didn't Go Well (Stop)

- **Swipe animation took longer than estimated** — PanGestureRecognizer edge cases (diagonal swipes, slow swipes) took an extra day. Should have spiked this in Sprint 1
- **API tests still only running locally** — CD is not set up yet, so the instructor cannot hit a live URL without us running the server. Need to fix this in Sprint 3

---

## What To Do Differently (Start)

- **Sprint 3: Set up CD to Azure App Service** — commit to having a live URL by Sprint 3 Day 3
- **Sprint 3: Spike any new UI interaction in the first 2 days** — don't let animation or gesture work block story completion in the back half of the sprint

---

## Action Items

| Action | Owner | By When |
|---|---|---|
| Set up CD workflow in GitHub Actions | [Member 2] | Sprint 3 Day 3 |
| Spike SignalR real-time connection in Day 1-2 | [Member 3] | Sprint 3 Day 2 |
| Update README with live Azure URL | Duane | Sprint 3 Day 4 |

---

## Actual Velocity for Yesterday's Weather (Sprint 3)

**Sprint 2 completed:** [X] story points → this becomes Sprint 3 forecast
