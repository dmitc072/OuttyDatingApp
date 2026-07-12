# Sprint 1 Planning

**Sprint Goal:** Deliver a working foundation where a user can register via Google SSO, create and delete a profile with photo, and set adventure type preferences and skill levels. All data persists in Azure.

**Sprint Dates:** July 6, 2026 – July 12, 2026 (1 week)

**Scrum Master:** Duane Mitchell
**Product Owner:** Jazmin Johnson
**Developer:** Yamani Barnes

---

## Velocity Forecast

**Forecast: 16 story points**

### Rationale

This is Sprint 1, our first sprint together as a team, so we have no historical velocity data to reference. We used the following reasoning to arrive at 16 points:

1. **Team capacity:** 3 members × approximately 8 hours per week of project time × 2 weeks = ~48 available hours this sprint
2. **Learning curve adjustment:** We are all new to .NET MAUI and Azure setup, so we applied a 40% reduction to account for ramp-up time, environment setup, and integration work that does not directly produce story points
3. **Story point scale:** On our scale, 1 point ≈ roughly 2–3 hours of focused work. With ~28 effective hours after adjustment, 16 points is a realistic but challenging target
4. **Must Have only:** We committed only to Must Have stories for Sprint 1. No Should Have or Could Have stories were pulled in

We will use this sprint's actual velocity to set a more accurate forecast for Sprint 2.

**Stories committed to Sprint 1:**

| ID        | Story                             | Points     |
| --------- | --------------------------------- | ---------- |
| US-01     | Google SSO Login                  | 3          |
| US-02     | Create Profile (name, photo, bio) | 3          |
| US-03     | Upload/Update Profile Photo       | 2          |
| US-04     | Delete Account                    | 2          |
| US-06     | Select Adventure Types            | 3          |
| US-07     | Set Skill Level per Activity      | 3          |
| **Total** |                                   | **16 pts** |

---

## Sprint Backlog — Stories Decomposed into Tasks

### US-01 — Google SSO Login (3 pts)

| Task                                                                        | Owner  | Estimate | Status      |
| --------------------------------------------------------------------------- | ------ | -------- | ----------- |
| Set up Microsoft Entra ID / Google OAuth app registration in Azure portal   | Jazmin | 2h       | In Process  |
| Install and configure Microsoft.Identity.Client NuGet package in Outty.App  | Jazmin | 1h       | TIn Process |
| Build LoginPage.xaml UI with Google Sign-In button                          | Jazmin | 1h       | In Process  |
| Implement OAuth token exchange and session persistence in AuthService.cs    | Jazmin | 2h       | In Process  |
| Write unit tests for AuthService (token valid, token expired, auth failure) | Duane  | 2h       | To Do       |

|

### US-02 — Create Profile (3 pts)

| Task                                                                     | Owner  | Estimate | Status     |
| ------------------------------------------------------------------------ | ------ | -------- | ---------- |
| Design Profile Page in Figma (name, bio, photo placeholder)              | Duane  | 2h       | In Process |
| Build Profile Page in MAUI matching Figma design                         | Jazmin | 2h       | In Process |
| Create User model in Outty.Core (UserId, Name, Bio, PhotoUrl, CreatedAt) | Jazmin | 1h       | In Process |
| Implement ProfileService.cs — CreateProfile() saves to Azure SQL via API | Jazmin | 2h       | In Process |
| Write unit tests for ProfileService.CreateProfile()                      | Duane  | 1h       | To Do      |

### US-03 — Upload/Update Profile Photo (2 pts)

| Task                                                            | Owner  | Estimate | Status     |
| --------------------------------------------------------------- | ------ | -------- | ---------- |
| Implement photo picker using MediaPicker in MAUI                | Jazmin | 1h       | In Process |
| Build PhotoService.cs — upload photo to Azure Blob Storage      | Jazmin | 2h       | In Process |
| Update profile record with new PhotoUrl after successful upload | Jazmin | 1h       | In Process |
| Write unit tests for PhotoService.UploadPhoto()                 | Duane  | 1h       | To Do      |

### US-04 — Delete Account (2 pts)

| Task                                                                                     | Owner  | Estimate | Status     |
| ---------------------------------------------------------------------------------------- | ------ | -------- | ---------- |
| Add Delete Account button to ProfilePage with confirmation dialog                        | Jazmin | 1h       | In Process |
| Implement AccountService.DeleteAccount() — removes user from Azure SQL, photos from Blob | Jazmin | 2h       | In Process |
| Redirect to LoginPage after successful deletion                                          | Jazmin | 0.5h     | In Process |
| Write unit tests for AccountService.DeleteAccount()                                      | Duane  | 1h       | To Do      |

### US-06 — Select Adventure Types (3 pts)

| Task                                                                    | Owner  | Estimate | Status     |
| ----------------------------------------------------------------------- | ------ | -------- | ---------- |
| Design Adventure Preferences Page in Figma (multi-select activity grid) | Duane  | 1h       | In Process |
| Build Adventure Preferences Page in MAUI                                | Jazmin | 2h       | In Process |
| Create Activity Preference model (UserId, ActivityType, SkillLevel)     | Jazmin | 0.5h     | In Process |
| Implement PreferencesService.SaveActivityPreferences()                  | Jazmin | 1.5h     | To Do      |
| Write unit tests for PreferencesService                                 | Duane  | 1h       | To Do      |

### US-07 — Set Skill Level per Activity (3 pts)

| Task                                                                                                            | Owner  | Estimate | Status |
| --------------------------------------------------------------------------------------------------------------- | ------ | -------- | ------ |
| Add skill level selector (Beginner / Intermediate / Advanced) per activity on PreferencesPage                   | Yamani | 2h       | To Do  |
| Update PreferencesService to save skill level alongside activity type                                           | Yamani | 1h       | To Do  |
| Validate at least one activity is selected before allowing profile completion                                   | Yamani | 1h       | To Do  |
| Write unit tests for skill level validation logic                                                               | Duane  | 1h       | To Do  |
| BDD test: Given a user selects Hiking at Intermediate level, When they save, Then their preference is persisted | Duane  | 1.5h     | To Do  |

---

## Kanban Board

**GitHub Projects Board URL:** https://github.com/users/dmitc072/projects/3

The Sprint Backlog is managed using GitHub Projects. Tasks move through the workflow: **To Do → In Progress → Done** as work is completed during the sprint.

---

## Definition of Done (Sprint 1)

A story is done when:

- [ ] All data persists correctly in Azure SQL / Blob Storage
- [ ] Unit tests for that story are written and passing
- [ ] Code is committed and pushed to the team's GitHub repository
- [ ] No critical bugs on the happy path
- [ ] At least one team member has tested the feature end-to-end
