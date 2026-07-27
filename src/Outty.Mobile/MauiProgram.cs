using Microsoft.Extensions.Logging;
using Outty.Mobile.Services;
using Outty.Mobile.Views;

namespace Outty.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont(
                    "OpenSans-Regular.ttf",
                    "OpenSansRegular");

                fonts.AddFont(
                    "OpenSans-Semibold.ttf",
                    "OpenSansSemibold");
            });

        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddSingleton(provider =>
        {
            return new HttpClient
            {
                BaseAddress = new Uri(ApiOptions.BaseAddress),
                Timeout = TimeSpan.FromSeconds(15)
            };
        });

builder.Services.AddSingleton<
    IMessagingService,
    MessagingService>();

        builder.Services.AddTransient<ConversationsPage>();
        builder.Services.AddTransient<ChatPage>();

        builder.Services.AddSingleton<GoogleAuthService>();
        builder.Services.AddSingleton<ApiClient>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<MatchingPage>();
        builder.Services.AddTransient<CreateProfilePage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<DeleteAccountPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}