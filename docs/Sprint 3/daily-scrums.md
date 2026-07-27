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

---

## Impediment Log

| Date | Impediment | Owner | Status | Resolution |
|---|---|---|---|---|
| Day 3 (Jul 22) | SQL firewall rejected connections after dev machine's public IP changed | Duane | Resolved | Added new firewall rule for the current IP |
| Day 3 (Jul 22) | Serverless DB cold-start caused a connection timeout on first request after idle | Duane | Known/Accepted | Retried successfully; not a bug, an expected serverless auto-pause behavior |
| Day 3 (Jul 22) | Azure SQL credentials (database password) not yet available, preventing API connection and end-to-end testing of messaging functionality | Jazmin | Open | Waiting for the completed Azure SQL connection credentials from the Scrum Master before configuring User Secrets and validating the messaging feature.|