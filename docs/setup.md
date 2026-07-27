# Developer Setup Guide

This guide explains how to set up the development environment for the Outty application. Follow these steps before building or running the project.

---

# System Requirements

Before you begin, install the following software:

- .NET 10 SDK
- .NET MAUI Workload
- Java 17
- Android Studio (for Android development)
- Git

---

# Clone the Repository

Clone the repository and navigate to the project folder.

```bash
git clone https://github.com/dmitc072/OuttyDatingApp.git
cd OuttyDatingApp
```

---

# Install .NET MAUI

Install the .NET MAUI workload (run once per machine).

```bash
sudo dotnet workload install maui
```

Verify the installation:

```bash
dotnet workload list
```

---

# Install Java 17

Android development requires Java 17.

macOS:

```bash
brew install --cask temurin@17
```

Verify the installation:

```bash
java -version
```

---

# Restore Project Dependencies

```bash
dotnet restore
```

---

# Install Android SDK Components

Run the following command to install the required Android SDK packages.

```bash
dotnet build src/Outty.Mobile \
-t:InstallAndroidDependencies \
-f net10.0-android \
-p:AndroidSdkDirectory=$HOME/Library/Developer/Xamarin/android-sdk-macosx \
-p:AcceptAndroidSDKLicenses=true
```

---

# Build the Project

```bash
dotnet build
```

---

# Configure Google Sign-In

Google Sign-In requires your own OAuth client from Google Cloud Console — the app won't build a working sign-in flow without it (it fails with a clear error instead of crashing, but Login won't work).

1. Go to [Google Cloud Console](https://console.cloud.google.com/) and create (or select) a project.
2. **APIs & Services → OAuth consent screen** (or **Google Auth Platform → Branding/Audience** on newer accounts) — configure the app name, user support email, and add the `.../auth/userinfo.email`, `.../auth/userinfo.profile`, and `openid` scopes.
3. **APIs & Services → Credentials → Create Credentials → OAuth client ID.**
4. Application type: **iOS** — not "Desktop app" or "Web application". Google blocks custom URI scheme redirects (like the one this app uses to get back from the browser) for those client types; iOS-type clients are the ones Google allowlists for it, even though we're targeting Android/Windows here.
5. **Bundle ID** can be anything (e.g. `com.outty.mobile`) — it doesn't need to be a real published iOS app.
6. After creation, open the client's details and copy the **iOS URL scheme** it shows you (something like `com.googleusercontent.apps.123456789`), plus the **Client ID**. iOS-type clients don't get a Client Secret — leave that blank.
7. Open `src/Outty.Mobile/Services/GoogleAuthOptions.cs` and fill in:

```csharp
public const string ClientId = "your-client-id.apps.googleusercontent.com";
public const string ClientSecret = "";
public const string RedirectUri = "com.googleusercontent.apps.123456789:/oauth-callback";
```

8. Also update `CallbackScheme` in `src/Outty.Mobile/Platforms/Android/WebAuthenticationCallbackActivity.cs` to the same iOS URL scheme (just the scheme, no path) — it must match `RedirectUri` above exactly, since Android uses it to route the browser's redirect back into the app.
9. Full rebuild + reinstall required after changing this (the scheme is baked into the Android manifest's intent filter, so an incremental/hot-reload deploy may not pick it up):
   ```bash
   dotnet build src/Outty.Mobile -f net10.0-android -t:Install
   ```
10. Don't commit real credentials to a public repo — if this repo is public, consider keeping your local copy of `GoogleAuthOptions.cs` untracked (`git update-index --assume-unchanged src/Outty.Mobile/Services/GoogleAuthOptions.cs`) or moving the values to a gitignored file.

---

# Running the Application

## Run the Web API

`Outty.Api` is deployed and live at **https://outty-api.azurewebsites.net** — the mobile app (`src/Outty.Mobile/Services/ApiOptions.cs`) points at this by default, so most people testing the app **don't need to run the API themselves at all**, including external/Windows testers off your network.

You only need to run it locally if you're actively developing/debugging the API itself:

```bash
dotnet run --project src/Outty.Api
```

If you do this, temporarily point `ApiOptions.BaseAddress` at your local instance instead — `http://10.0.2.2:5157` from the Android emulator (10.0.2.2 is the emulator's alias for your host machine's localhost), or `http://localhost:5157` from Windows/other. Running it locally also needs a working Azure Key Vault connection (via `DefaultAzureCredential`) for the SQL connection string, so make sure you're logged into Azure CLI (`az login`) with access to the `outty-kv` Key Vault first.

---

## Run the Mobile Application

### Android Device

1. Enable **Developer Options**.
2. Enable **USB Debugging**.
3. Connect the device using a USB cable.
4. Run:

```bash
dotnet run --project src/Outty.Mobile -f net10.0-android
```

---

## Android Emulator

1. Install **Android Studio** from https://developer.android.com/studio.
2. Open Android Studio.
3. On the **Welcome** screen, select **More Actions → Virtual Device Manager**.

![Android Studio Welcome Screen](images/android-studio-welcome.png)

4. Click **Create Device**.
5. Choose a device (recommended: **Pixel 10**).
6. Select a **Google Play** system image.
7. Finish the setup and start the emulator.

Run the mobile application:

```bash
dotnet run --project src/Outty.Mobile -f net10.0-android
```

> **Recommendation:** Use a Google Play system image. During development, the **Pixel 10** emulator provided the best compatibility for image selection and other Android features.

---

### Windows

Run the Windows version of the application on a Windows machine.

```bash
dotnet run --project src/Outty.Mobile -f net10.0-windows10.0.19041.0
```

---

# Troubleshooting

| Problem                           | Solution                                                                    |
| --------------------------------- | --------------------------------------------------------------------------- |
| Android SDK not found             | Install the Android SDK dependencies and rebuild the project.               |
| Android SDK licenses not accepted | Add `-p:AcceptAndroidSDKLicenses=true` to the build command.                |
| `project.assets.json` missing     | Run `dotnet restore`.                                                       |
| Java version error                | Install Java 17 and verify it is the active version.                        |
| Photo picker not working          | Use an emulator with a **Google Play** system image (Pixel 10 recommended). |
| MAUI workload missing             | Run `sudo dotnet workload install maui`.                                    |
| "Google OAuth Client ID is not configured" | Fill in `src/Outty.Mobile/Services/GoogleAuthOptions.cs` (see Configure Google Sign-In above). |
| Login/Create Profile fails to reach server | Make sure `dotnet run --project src/Outty.Api` is running locally and that you're logged into Azure CLI for Key Vault access. |

---

# Helpful Resources

- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core/)
- [Android Studio](https://developer.android.com/studio)
- [GitHub Actions Documentation](https://docs.github.com/actions)

---

If you experience additional setup issues, consult the official documentation above or contact a member of Team Outty.
