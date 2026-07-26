# Outty – Outdoor Adventure Matching App

**Course:** SWE 6733 – Emerging Software Engineering Processes

Outty is a cross-platform mobile application that helps people find outdoor adventure partners based on shared interests, experience level, and location. The application allows users to create profiles, discover potential matches, and connect with others who enjoy activities such as hiking, kayaking, climbing, backpacking, and more.

---

## Repository

- **Source Code:** https://github.com/dmitc072/OuttyDatingApp
- **GitHub Project Board:** https://github.com/users/dmitc072/projects/3

---

## Team Outty

| Member         | Scrum Role    |
| -------------- | ------------- |
| Duane Mitchell | Scrum Master  |
| Jazmin Johnson | Product Owner |
| Yamani Barnes  | Developer     |

---

## Technology Stack

- .NET MAUI
- ASP.NET Core Web API
- Azure SQL Database
- GitHub Projects
- GitHub Actions
- Figma

---

## Project Artifacts

| Artifact                   | Location                                                        |
| -------------------------- | --------------------------------------------------------------- |
| Product Vision             | [docs/product-vision.md](docs/product-vision.md)                |
| Product Backlog            | [GitHub Projects](https://github.com/users/dmitc072/projects/3) |
| Backlog Ordering Rationale | [docs/backlog-rationale.md](docs/backlog-rationale.md)          |
| Definition of Ready        | [docs/definition-of-ready.md](docs/definition-of-ready.md)      |
| Sprint Documentation       | [docs](docs/)                                                   |
| UI Designs                 | [docs/design.md](docs/design.md)                                |

---

## Repository Structure

```
OuttyDatingApp/
├── docs/                  # Project documentation
├── src/
│   ├── Outty.Mobile/      # .NET MAUI mobile application
│   ├── Outty.Api/         # ASP.NET Core Web API
│   └── Outty.Shared/      # Shared models and classes
├── sprint-1/
├── sprint-2/
├── sprint-3/
└── Outty.sln
```

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- .NET MAUI workload
- Java 17 (Android development)
- Android SDK (Android development)

For detailed installation instructions, emulator setup, and troubleshooting, see the [Developer Setup Guide](docs/setup.md).

---

## Build

Restore project dependencies:

```bash
dotnet restore
```

Build the solution:

```bash
dotnet build
```

---

## Run the API

```bash
dotnet run --project src/Outty.Api
```

---

## Run the Mobile App

### Android

```bash
dotnet run --project src/Outty.Mobile -f net10.0-android
```

### Windows

```bash
dotnet run --project src/Outty.Mobile -f net10.0-windows10.0.19041.0
```

---

## Features

Current and planned functionality includes:

- User account creation
- Google Sign-In
- User profile management
- Outdoor activity preferences
- Swipe-based matching
- Match notifications
- Search radius preferences
- Availability scheduling
- Azure SQL data storage

---

## Development Process

This project follows Scrum practices as part of the SWE 6733 course. Development includes:

- Product Backlog
- Sprint Planning
- Sprint Reviews
- Sprint Retrospectives
- GitHub Projects for backlog management
- Continuous integration using GitHub Actions

---

## AI Usage

AI tools were used to assist with brainstorming, planning, document organization, and code suggestions. All generated content was reviewed, validated, and modified by the development team before submission.

---

## License

This repository was created for educational purposes as part of the SWE 6733 – Emerging Software Engineering Processes course at Kennesaw State University.
