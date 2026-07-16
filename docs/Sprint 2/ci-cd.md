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

**File location in repo:** `.github/workflows/ci.yml`

> The workflow builds/tests `Outty.Api` (which pulls in `Outty.Shared` via project reference) rather than the full `OuttyDatingApp.slnx`. `Outty.Mobile` is a .NET MAUI project and needs the MAUI workload installed on the runner (`dotnet workload install maui-android`) before it can build on `ubuntu-latest` — it's left out of this workflow for now.

```yaml
name: Outty CI

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Set up .NET 10
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore dependencies
        run: dotnet restore src/Outty.Api/Outty.Api.csproj

      - name: Build solution
        run: dotnet build src/Outty.Api/Outty.Api.csproj --no-restore --configuration Release

      - name: Run all tests
        run: dotnet test src/Outty.Api/Outty.Api.csproj --no-build --configuration Release --verbosity normal

      - name: Upload test results
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: test-results
          path: '**/TestResults/*.xml'
```

---

## How to Set Up the CI Workflow

**Step 1 — Create the workflow file in your repo:**

```bash
# From the repo root
mkdir -p .github/workflows
touch .github/workflows/ci.yml
```

**Step 2 — Paste the YAML above into ci.yml**

```bash
code .github/workflows/ci.yml
# Paste the workflow YAML, save
```

**Step 3 — Commit and push:**

```bash
git add .github/workflows/ci.yml
git commit -m "Add GitHub Actions CI workflow — builds and tests on every push"
git push origin main
```

**Step 4 — Verify it ran:**

1. Go to your GitHub repo
2. Click the **Actions** tab
3. You should see a workflow run called "Outty CI"
4. Click it → should show green checkmarks for all three steps

---

## CI Run Evidence

**Actions tab URL:** https://github.com/dmitc072/OuttyDatingApp/actions

![CI Pipeline Green](../assets/sprint2-ci-passing.png)
> Replace with actual screenshot of your GitHub Actions run showing all steps passing.

---

## What CI Catches Automatically

Every push triggers:

| Check | What it catches |
|---|---|
| `dotnet restore` | Missing or broken NuGet packages |
| `dotnet build` | Compilation errors in any project |
| `dotnet test` | Any failing unit or BDD test |

If any step fails, GitHub marks the commit with a red ✗ and sends an email notification to the committer. No broken code reaches `main` undetected.

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
          dotnet-version: '10.0.x'

      - name: Publish API
        run: dotnet publish src/Outty.Api/Outty.Api.csproj -c Release -o ./publish

      - name: Deploy to Azure App Service
        uses: azure/webapps-deploy@v3
        with:
          app-name: 'outty-api'
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
