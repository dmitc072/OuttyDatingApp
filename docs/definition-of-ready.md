# Definition of Ready

A Product Backlog Item (PBI) is considered **Ready** to enter a sprint only when ALL of the following criteria are met. No story enters sprint planning without satisfying every item on this list.

---

## Required Fields

### a) Title
A short, descriptive label that uniquely identifies the feature.

**Format:** `[Feature area]: [What it does]`

**Example:** `Authentication: Google SSO Login`

---

### b) User Story Opening Sentence
Written in the standard user story format:

**Format:** `As a [user type], I want to [action] so that [benefit].`

**Example:** `As a user, I want to log in using my Google account so that I don't need to create a separate Outty password.`

The opening sentence must:
- Identify a specific user type (not just "user" where a more specific type applies)
- Describe one action, not multiple
- State a clear benefit that explains the value delivered

---

### c) Additional Details (Acceptance Criteria)
Testable conditions that define what "done" looks like for this story. Written as a checklist of observable behaviors.

**Format:** Each criterion starts with a verb and describes something that can be manually tested.

**Example for Google SSO Login:**
- User taps "Sign in with Google" and is redirected to Google OAuth
- After successful Google authentication, user is returned to the Outty home screen
- Auth token persists across app restarts (user stays logged in)
- If Google auth fails, user sees an error message and can retry
- User cannot access any other screen without completing login

A story with vague or untestable acceptance criteria is **not Ready**.

---

### d) Estimated in Story Points
Story points assigned by the full team using planning consensus. No story enters a sprint unestimated.

**Scale:**

| Points | Meaning | Example |
|---|---|---|
| 1 | Trivial — less than a few hours | Change a button label |
| 2 | Small — half a day | Add a form field with validation |
| 3 | Medium — 1–2 days | Build a profile screen with data persistence |
| 5 | Large — 3–4 days | Build the swipe UI with gesture recognition |
| 8 | Very large — consider splitting | Full matching algorithm end-to-end |

Stories estimated at 8 points should be reviewed for splitting before entering a sprint.

---

### e) Priority Assigned
MoSCoW priority must be set:

| Priority | Meaning |
|---|---|
| Must Have | Required for the sprint goal. Sprint fails without it. |
| Should Have | High value but not sprint-blocking. |
| Could Have | Nice to have. Only included if capacity allows. |

---

### f) Dependencies Identified
Any stories that must be completed before this one can start are listed by ID.

**Example:** `Depends on: US-01 (Login), US-02 (Profile Creation)`

If a dependency is not yet complete, the story is **not Ready** regardless of other criteria.

---

## Definition of Ready Checklist

Before a story enters sprint planning, the Product Owner confirms:

- [ ] Title is clear and specific
- [ ] User story opening sentence follows the format
- [ ] Acceptance criteria are written and testable
- [ ] Story is estimated in story points
- [ ] MoSCoW priority is assigned
- [ ] Dependencies are listed and resolved

---

## Example — Fully Ready Story

**ID:** US-01

**Title:** Authentication: Google SSO Login

**User Story:** As a user, I want to register and log in using my Google account so that I don't need to create a separate Outty password.

**Acceptance Criteria:**
- User taps "Sign in with Google" and is redirected to Google OAuth screen
- After successful authentication, user lands on the Outty home screen
- Auth token persists — user remains logged in across app restarts
- If Google auth fails, user sees a clear error message with a retry option
- Unauthenticated users cannot access any screen beyond the login page

**Story Points:** 3

**Priority:** Must Have

**Sprint:** 1

**Dependencies:** None — this is the first story in the dependency chain
