namespace Outty.Shared.Utilities;

public static class SearchRadiusValidator
{
    public const int MinimumMiles = 5;
    public const int MaximumMiles = 100;
    public const int DefaultMiles = 25;

    public static bool IsValid(int miles) =>
        miles >= MinimumMiles && miles <= MaximumMiles;

    public static int Normalize(int? requestedMiles) =>
        requestedMiles is null
            ? DefaultMiles
            : Math.Clamp(requestedMiles.Value, MinimumMiles, MaximumMiles);
}
