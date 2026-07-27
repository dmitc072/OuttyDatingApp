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
                BaseAddress = new Uri("https://localhost:7001/"),
                Timeout = TimeSpan.FromSeconds(15)
            };
        });

builder.Services.AddSingleton<
    IMessagingService,
    MessagingService>();

        builder.Services.AddTransient<ConversationsPage>();
        builder.Services.AddTransient<ChatPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}