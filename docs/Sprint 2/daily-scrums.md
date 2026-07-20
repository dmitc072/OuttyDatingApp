# Sprint 2 — Daily Scrum Logs

Daily scrums held asynchronously via Discord every morning at 9:00 AM. Each member posts their three items. Scrum Master (Duane) flags impediments and follows up on blockers same day.

---

## Daily Scrum — Day 1 | July 13, 2026

### Duane Mitchell (Scrum Master)

**What did I do in the last 24 hours?**
Facilitated Sprint 2 planning — finalized the 17-point commitment (US-08 through US-12), assigned task ownership, and confirmed team availability per the Sprint 1 retro action items.

**What will I do in the next 24 hours?**
Kick off US-12 database setup — design the Azure SQL schema for Match, Swipe, and Preference tables.

**Any impediments?**
None.

---

### Jazmin Johnson (Product Owner)

**What did I do in the last 24 hours?**
Create UI for the following pages: Login, Create Profile, Home Page

**What will I do in the next 24-48 hours?**
-Create UI for the following pages: Setting, Delete Account
**Any impediments?**

---

### Yamani Barnes (Developer)

**What did I do in the last 24 hours?**

**What will I do in the next 24 hours?**

**Any impediments?**

---

## Daily Scrum — Day 4 | July 16, 2026

### Duane Mitchell (Scrum Master)

**What did I do in the last 24 hours?**
Wrapped up US-12 database setup (schema, EF Core migrations, unit tests — done as of Day 3). Set up the GitHub Actions CI workflow directly on GitHub (`.github/workflows/main.yml`) and committed it to `dev`; later added the MAUI Android workload install step so `Outty.Mobile` builds in CI too, not just `Outty.Api`/`Outty.Shared`.

**What will I do in the next 24 hours?**
Start the Figma design for SwipePage (US-10) and support Yamani on the PanGestureRecognizer implementation.

**Any impediments?**
None. CI pipeline green on `dev`.

---

### Jazmin Johnson (Product Owner)

**What did I do in the last 24 hours?**
-Create UI for the following pages: Setting, Delete Account

**What will I do in the next 24 hours?**

**Any impediments?**

---

### Yamani Barnes (Developer)

**What did I do in the last 24 hours?**

**What will I do in the next 24 hours?**

**Any impediments?**

---

## Daily Scrum — Day 7 | July 19, 2026

### Duane Mitchell (Scrum Master)

**What did I do in the last 24 hours?**
Supported continued work on US-10/US-11 (still in progress). Prepared the Sprint 2 review/retro and updated the burndown chart to reflect actual progress.

**What will I do in the next 24 hours?**
Run Sprint 2 review and retrospective; carry US-08, US-09, US-10, and US-11 into Sprint 3 planning.

**Any impediments?**
US-08 and US-09 never started; US-10 and US-11 still in progress — carrying into Sprint 3. Photo upload also isn't working on the Android emulator (works on Windows).

---

### Jazmin Johnson (Product Owner)

**What did I do in the last 24 hours?**

**What will I do in the next 24 hours?**
-Create UI for In-App Messaging and the Read Receipts features
-Fix connections to the backend for storage
-Make sure the photo pieces work in the android simulation

**Any impediments?**

---

### Yamani Barnes (Developer)

**What did I do in the last 24 hours?**

**What will I do in the next 24 hours?**

**Any impediments?**

---

## Impediment Log

| Date           | Impediment                                                                         | Owner  | Status | Resolution                                  |
| -------------- | ---------------------------------------------------------------------------------- | ------ | ------ | ------------------------------------------- |
| Day 7 (Jul 19) | Photo upload not working in Android emulator, though it works in the Windows build | Jazmin | Open   | Carried into Sprint 3 — needs investigation |

---

## Pair Programming Evidence

**Planned Pairing Session 1 — Day [X]**
https://youtu.be/Uvse9UbsL84
