# Sprint 3 — Daily Scrum Logs

Daily scrums held asynchronously via Discord every morning at 9:00 AM. Each member posts their three items. Scrum Master (Duane) flags impediments and follows up on blockers same day.

---

## Daily Scrum — Day 3 | July 22, 2026

### Duane Mitchell (Scrum Master)

**What did I do in the last 24 hours?**
Implemented the US-11 Matching Algorithm backend: `MatchingService.GetCandidates()` (filters by shared interests + state, excludes self, orders by shared-interest count), exposed via `GET /matches/candidates/{profileId}`. Also implemented US-08's `SearchRadiusValidator` (5–100 mile validation, defaults to 25) and `PUT /profiles/{id}/search-radius`. Added the `SearchRadiusMiles` column to `dbo.Profiles` and re-scaffolded EF models. Wrote 22 new tests (19 unit + 3 BDD/A-TDD) covering both — full suite now 44/44 passing, wired into CI.

**What will I do in the next 24 hours?**
Support Yamani on connecting `SwipePage` to the new `/matches/candidates` endpoint, and Jazmin on wiring the search radius slider to `/profiles/{id}/search-radius`.

**Any impediments?**
The SQL server firewall had to be updated twice this sprint — once for the dev machine's IP changing, once for a serverless cold-start connection timeout. Neither blocked work for more than a few minutes, but worth remembering firewall rules may need refreshing again before Sprint 4.

---

### Jazmin Johnson (Product Owner)

**What did I do in the last 24 hours?**
Continued refining the profile creation experience by planning the implementation of City, State, and ZIP Code fields to support future location-based matching.

Collaborated with the team on the search radius feature and discussed the best approach for storing and using location data for matching users.

Investigated the Azure SQL database configuration and confirmed the server connection information. Verified that the remaining blocker for the messaging feature is obtaining the database credentials to complete the connection.

**What will I do in the next 24 hours?**
Implement the location fields within the profile creation and profile display pages.

Begin wiring the search radius setting in the mobile application to the backend endpoint once it is available.

Continue testing profile functionality and complete messaging integration after the Azure SQL credentials are received.

**Any impediments?**
Still waiting on the Azure SQL database password to configure the API connection and complete end-to-end testing of the messaging feature. Until credentials are received, database-dependent features cannot be fully validated

---

### Yamani Barnes (Developer)

**What did I do in the last 24 hours?**

**What will I do in the next 24 hours?**

**Any impediments?**

None reported.

---

---

## Daily Scrum — Day 6 | July 25, 2026

### Duane Mitchell (Scrum Master)

**What did I do in the last 24 hours?**
Completed testing for the MatchingService and SearchRadiusValidator, verified all 44 automated tests were passing, validated the API against Azure SQL, and confirmed the GitHub Actions CI pipeline completed successfully.

**What will I do in the next 24 hours?**
Finalize Sprint 3 documentation, review completed stories, and prepare the Sprint Review demonstration.

**Any impediments?**
None.

---

### Jazmin Johnson (Product Owner)

**What did I do in the last 24 hours?**
Continued refining the profile creation experience by planning the implementation of City, State, and ZIP Code fields. Continued reviewing the search radius feature and database configuration while preparing for future messaging integration.

**What will I do in the next 24 hours?**
Continue implementing profile location fields, assist with validating the search radius workflow, and participate in the Sprint Review.

**Any impediments?**
Still waiting on the Azure SQL database credentials needed to complete end-to-end messaging validation.

---

### Yamani Barnes (Developer)

**What did I do in the last 24 hours?**

**What will I do in the next 24 hours?**

**Any impediments?**

None reported.

---

## Daily Scrum — Day 7 | July 26, 2026

### Duane Mitchell (Scrum Master)

**What did I do in the last 24 hours?**
Completed Sprint 3 deliverables, verified Azure deployment, confirmed all completed stories met the Definition of Done, and prepared the Sprint Review and Retrospective documentation.

**What will I do in the next 24 hours?**
Begin Sprint 4 planning and prepare work for the remaining UI stories.

**Any impediments?**
None.

---

### Jazmin Johnson (Product Owner)

**What did I do in the last 24 hours?**
Reviewed Sprint 3 progress, verified completed backlog items, and continued planning the remaining profile and messaging features for Sprint 4.

**What will I do in the next 24 hours?**
Refine the Sprint 4 backlog and continue work on profile management and messaging functionality.

**Any impediments?**
Waiting on Azure SQL credentials before completing messaging integration.

---

### Yamani Barnes (Developer)

**What did I do in the last 24 hours?**

**What will I do in the next 24 hours?**

**Any impediments?**

None reported.

---

## Daily Scrum — Day 8 | July 27, 2026

### Duane Mitchell (Scrum Master)

**What did I do in the last 24 hours?**
Merged Jazmin's profile/messaging rewrite with the Google OAuth work — resolved conflicts in `MauiProgram.cs` and `CreateProfilePage`, keeping Jazmin's UI (multi-photo, search radius slider) while wiring in the real `/profiles` API call. While fixing the resulting build, found and fixed two pre-existing gaps in the messaging feature: `Conversation`/`ConversationParticipant`/`Message` were never added as `DbSet`s on `OuttyDbContext`, and `Program.cs` never called `AddControllers()`/`MapControllers()`, so every messaging route 404'd. Added the missing `Fishing`/`Running`/`Other` interests to the database to match what the UI actually offers. Deployed `Outty.Api` to a real Azure App Service (`outty-api.azurewebsites.net`) so the team and external testers no longer need a local API running. Added 5 dummy users/profiles to the live database for manual testing, and 18 automated tests for `MessagingController`. Fixed the "Advanced"/"Expert" experience-level mismatch (database only has Beginner/Intermediate/Advance) by restoring per-interest experience pickers. Fixed 6 Shell navigation crashes caused by `AppShell.xaml`'s `HomePage`/`ConversationsPage`/`ProfilePage` now living inside a `TabBar`, which changed how those routes need to be addressed vs. the "global" routes.

**What will I do in the next 24 hours?**
Verify the Windows OAuth callback handling (written from Microsoft's documented pattern but never compile-checked, since this Mac can't target Windows) once a Windows machine is available. Follow up with Jazmin on the messaging feature now that it's actually reachable.

**Any impediments?**
None blocking. Flagging for visibility: the Windows sign-in flow is unverified, and the search radius/distance/goals fields aren't collected in the current Create Profile UI (dropped when merging in Jazmin's version) — worth a decision on whether to add them back.

---

### Jazmin Johnson (Product Owner)

**What did I do in the last 24 hours?**

**What will I do in the next 24 hours?**

**Any impediments?**

---

### Yamani Barnes (Developer)

**What did I do in the last 24 hours?**

**What will I do in the next 24 hours?**

**Any impediments?**

---

## Impediment Log

| Date           | Impediment                                                                                                                               | Owner  | Status         | Resolution                                                                                                                                             |
| -------------- | ---------------------------------------------------------------------------------------------------------------------------------------- | ------ | -------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Day 3 (Jul 22) | SQL firewall rejected connections after dev machine's public IP changed                                                                  | Duane  | Resolved       | Added new firewall rule for the current IP                                                                                                             |
| Day 3 (Jul 22) | Serverless DB cold-start caused a connection timeout on first request after idle                                                         | Duane  | Known/Accepted | Retried successfully; not a bug, an expected serverless auto-pause behavior                                                                            |
| Day 3 (Jul 22) | Azure SQL credentials (database password) not yet available, preventing API connection and end-to-end testing of messaging functionality | Jazmin | Open           | Waiting for the completed Azure SQL connection credentials from the Scrum Master before configuring User Secrets and validating the messaging feature. |
