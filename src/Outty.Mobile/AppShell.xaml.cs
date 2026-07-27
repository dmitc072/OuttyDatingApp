using Microsoft.Extensions.DependencyInjection;
using Outty.Mobile.Views;

namespace Outty.Mobile;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        Routing.RegisterRoute(
            nameof(CreateProfilePage),
            typeof(CreateProfilePage));

        Routing.RegisterRoute(
            nameof(SettingsPage),
            typeof(SettingsPage));

        Routing.RegisterRoute(
            nameof(DeleteAccountPage),
            typeof(DeleteAccountPage));

        Routing.RegisterRoute(
            nameof(ChatPage),
            typeof(ChatPage));

        MessagesShellContent.ContentTemplate =
            new DataTemplate(() =>
                serviceProvider.GetRequiredService<ConversationsPage>());
    }
}