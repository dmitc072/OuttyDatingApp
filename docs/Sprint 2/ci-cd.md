# Continuous Integration (CI) — Outty

## CI Tool: GitHub Actions

**CI Pipeline URL:** https://github.com/dmitc072/OuttyDatingApp/actions

---

## Why GitHub Actions?

We chose GitHub Actions as our CI tool for the following reasons:

1. **Zero additional setup cost** — GitHub Actions is built directly into GitHub, which is already our required code management tool. No separate account, no separate dashboard, no third-party service to configure.

2. **Triggers automatically on push** — Every push to `main` and every pull request automatically triggers a build and test run. No manual steps required from any team member.

3. **Free for public repos** — Our repo is public (required by the course), so GitHub Actions runs are completely free with no usage limits on the free tier.

4. **Native .NET support** — GitHub Actions has first-class support for .NET via the `actions/setup-dotnet` action. Building and testing a .NET 10 solution requires only a few lines of YAML — no complex configuration.

5. **Visible to everyone including the instructor** — The Actions tab on our public repo shows every CI run, its status, build logs, and test results. Anyone with the repo URL can see proof that CI is running.

6. **Integrates with GitHub Projects** — Failed CI runs can be linked to issues and pull requests, making it easy to track which commit broke the build.

---

## CI Workflow File

**File location in repo:** `.github/workflows/main.yml`

> The workflow restores/builds the full `OuttyDatingApp.slnx`, including `Outty.Mobile` (.NET MAUI, Android target). Since `ubuntu-latest` doesn't ship the MAUI SDK workload by default, a dedicated step installs it (`dotnet workload install maui-android`) before restore. This adds a few minutes to every run — worth it once Mobile code is actively changing; `dotnet test` targets `tests/Outty.Shared.Tests`, the project holding the test suite (see `tests.md`).

```yaml
name: Outty CI

on:
  push:
    branches: [main, dev]
  pull_request:
    branches: [main, dev]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Set up .NET 10
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "10.0.x"

      - name: Install MAUI Android workload
        run: dotnet workload install maui-android

      - name: Restore dependencies
        run: dotnet restore OuttyDatingApp.slnx

      - name: Build solution
        run: dotnet build OuttyDatingApp.slnx --no-restore --configuration Release

      - name: Run all tests
        run: dotnet test tests/Outty.Shared.Tests/Outty.Shared.Tests.csproj --no-build --configuration Release --verbosity normal

      - name: Upload test results
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: test-results
          path: "**/TestResults/*.xml"
```

---

## How This Was Set Up

The workflow above was added directly through the GitHub website (Actions tab → "set up a workflow yourself") and committed straight to `.github/workflows/main.yml` on the `dev` branch. Steps, for reference or to replicate on another repo:

1. Go to the repo → **Actions** tab → **"set up a workflow yourself"**
2. Name the file `.github/workflows/main.yml`, paste in the YAML above
3. Commit directly to `dev` (or open a PR if you prefer review-before-merge)
4. Check the **Actions** tab — a run called "Outty CI" appears and shows a green checkmark once `build-and-test` passes

---

## CI Run Evidence

**Actions tab URL:** https://github.com/dmitc072/OuttyDatingApp/actions

**Passing run:** https://github.com/dmitc072/OuttyDatingApp/actions/runs/29519579818 — `Outty CI` on `dev`, `build-and-test` job succeeded in 3m5s (2026-07-16), including the MAUI Android workload install and a full-solution build (`Outty.Api`, `Outty.Shared`, `Outty.Mobile`).

---

## What CI Catches Automatically

Every push triggers:

| Check            | What it catches                   |
| ---------------- | --------------------------------- |
| `dotnet restore` | Missing or broken NuGet packages  |
| `dotnet build`   | Compilation errors in any project |
| `dotnet test`    | Any failing unit or BDD test      |

If any step fails, GitHub marks the commit with a red ✗ and sends an email notification to the committer. No broken code reaches `dev` or `main` undetected.

> This is currently advisory only — failing CI doesn't block a merge. To make it enforced, add `build-and-test` as a required status check under branch protection for `dev`/`main` (Settings → Branches → Branch protection rules).

---

## Optional: Continuous Deployment (CD)

To also auto-deploy to Azure App Service on every successful CI run, add this job to the workflow after the test job:

```yaml
deploy-to-azure:
  needs: build-and-test
  runs-on: ubuntu-latest
  if: github.ref == 'refs/heads/main' && github.event_name == 'push'

  steps:
    - name: Checkout code
      uses: actions/checkout@v4

    - name: Set up .NET 10
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: "10.0.x"

    - name: Publish API
      run: dotnet publish src/Outty.Api/Outty.Api.csproj -c Release -o ./publish

    - name: Deploy to Azure App Service
      uses: azure/webapps-deploy@v3
      with:
        app-name: "outty-api"
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

**To enable CD:**

1. Go to Azure Portal → your App Service → Download publish profile
2. Go to GitHub repo → Settings → Secrets → New repository secret
3. Name: `AZURE_WEBAPP_PUBLISH_PROFILE`, Value: paste the downloaded file contents
4. Add the deploy job above to your workflow YAML
5. Push to main — CI runs tests, then CD deploys to Azure automatically

**Live API URL after CD setup:** https://outty-api.azurewebsites.net/swagger
