# Outty – Outdoor Adventure Matching App

**Course:** SWE 6733 – Emerging Software Engineering Processes

Outty is a mobile application that connects people interested in outdoor activities based on shared interests, experience level, and location. The goal is to make it easier for users to find compatible adventure partners for activities such as hiking, kayaking, climbing, and backpacking.

---

## Repository

This repository contains all project documentation required for Product Inception and Planning, including the Product Vision, Product Backlog, Backlog Ordering Rationale, and Definition of Ready.

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

| Artifact                   | Location                                                         |
| -------------------------- | ---------------------------------------------------------------- |
| Product Vision             | [docs/product-vision.md](docs/product-vision.md)                 |
| Product Backlog            | [GitHub Projects] (https://github.com/users/dmitc072/projects/3) |
| Backlog Ordering Rationale | [docs/backlog-rationale.md](docs/backlog-rationale.md)           |
| Definition of Ready        | [docs/definition-of-ready.md](docs/definition-of-ready.md)       |

---

## Repository Structure

```
OuttyDatingApp/
├── Outty.sln
├── src/
│   ├── Outty.Mobile/               # .NET MAUI app (iOS & Android)
│   │   ├── MauiProgram.cs
│   │   ├── MainPage.xaml
│   │   ├── AppShell.xaml
│   │   └── Platforms/
│   │       ├── Android/
│   │       ├── iOS/
│   │       ├── MacCatalyst/
│   │       └── Windows/
│   ├── Outty.Api/                  # ASP.NET Core Web API
│   │   └── Program.cs
│   └── Outty.Shared/               # Shared models & enums
│       ├── Models/
│       │   ├── User.cs
│       │   ├── Match.cs
│       │   └── Activity.cs
│       └── Enums/
│           └── ActivityType.cs
├── docs/
├── design/
├── sprint-1/
├── sprint-2/
└── sprint-3/
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- .NET MAUI workload (run once per machine):
  ```bash
  sudo dotnet workload install maui
  ```
- **Java 17** (required for Android builds):
  ```bash
  brew install --cask temurin@17
  ```

> **Note:** iOS builds require a Mac with Xcode installed. This project targets Android and Windows only to keep the team unblocked.

---

### First-time setup (run once after cloning)

**1. Restore dependencies:**

```bash
dotnet restore
```

**2. Install Android SDK dependencies:**

```bash
dotnet build src/Outty.Mobile -t:InstallAndroidDependencies -f net10.0-android \
  -p:AndroidSdkDirectory=$HOME/Library/Developer/Xamarin/android-sdk-macosx \
  -p:AcceptAndroidSDKLicenses=true
```

---

### Build the solution

```bash
dotnet build
```

### Run the API

```bash
dotnet run --project src/Outty.Api
```

### Run the Mobile App

```bash
# Android (Mac or Windows)
dotnet build src/Outty.Mobile -f net10.0-android

# Windows only — run this on a Windows machine
dotnet build src/Outty.Mobile -f net10.0-windows10.0.19041.0
```

---

### Common errors and fixes

| Error                                                   | Fix                                                         |
| ------------------------------------------------------- | ----------------------------------------------------------- |
| `XA5207: Could not find android.jar for API level 36`   | Run the Android SDK install command above                   |
| `Android SDK license agreements were not accepted`      | Add `-p:AcceptAndroidSDKLicenses=true` to the build command |
| `NETSDK1005: project.assets.json doesn't have a target` | Run `dotnet restore` first                                  |
| `Inadequate permissions` on workload install            | Use `sudo dotnet workload install maui`                     |
| Java version error (needs 17, has 11)                   | Run `brew install --cask temurin@17`                        |

---

## AI Usage

AI tools were used to assist with brainstorming, planning, and document organization. All AI-generated content was reviewed, revised, and validated by the team before submission.
