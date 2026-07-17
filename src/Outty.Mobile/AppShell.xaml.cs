using Outty.Mobile.Views;

namespace Outty.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(
            nameof(CreateProfilePage),
            typeof(CreateProfilePage));

        Routing.RegisterRoute(
            nameof(ProfilePage),
            typeof(ProfilePage));

        Routing.RegisterRoute(
            nameof(SettingsPage),
            typeof(SettingsPage));

        Routing.RegisterRoute(
            nameof(DeleteAccountPage),
            typeof(DeleteAccountPage));
    }
}