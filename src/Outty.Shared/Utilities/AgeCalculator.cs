namespace Outty.Shared.Utilities;

public static class AgeCalculator
{
    public const int MinimumAge = 18;

    public static int CalculateAge(DateTime birthDate, DateTime today)
    {
        var age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    public static bool IsAtLeast18(DateTime birthDate, DateTime today) =>
        CalculateAge(birthDate, today) >= MinimumAge;
}
