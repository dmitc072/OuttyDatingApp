using Outty.Shared.Utilities;

namespace Outty.Shared.Tests;

public class AgeCalculatorTests
{
    [Theory]
    [InlineData("2008-07-19", "2026-07-19", 18)]  // birthday is today
    [InlineData("2008-07-20", "2026-07-19", 17)]  // turns 18 tomorrow
    [InlineData("2008-07-18", "2026-07-19", 18)]  // turned 18 yesterday
    [InlineData("2026-07-19", "2026-07-19", 0)]   // born today
    [InlineData("2006-07-19", "2026-07-19", 20)]  // birthday is today, older
    [InlineData("1926-07-19", "2026-07-19", 100)] // birthday is today, 100 years
    [InlineData("2008-01-01", "2026-07-19", 18)]  // birthday already passed this year
    [InlineData("2008-12-31", "2026-07-19", 17)]  // birthday hasn't happened yet this year
    [InlineData("2004-02-29", "2026-02-28", 21)]  // leap-day birthday, day before anniversary in a non-leap year
    [InlineData("2004-02-29", "2026-03-01", 22)]  // leap-day birthday, day after anniversary in a non-leap year
    [InlineData("2025-07-19", "2026-07-19", 1)]   // exactly one year old
    [InlineData("2025-07-20", "2026-07-19", 0)]   // one day short of turning 1
    public void CalculateAge_ReturnsExpectedAge(string birthDateText, string todayText, int expectedAge)
    {
        var birthDate = DateTime.Parse(birthDateText);
        var today = DateTime.Parse(todayText);

        var age = AgeCalculator.CalculateAge(birthDate, today);

        Assert.Equal(expectedAge, age);
    }

    [Theory]
    [InlineData("2008-07-19", "2026-07-19", true)]   // exactly 18 today
    [InlineData("2008-07-20", "2026-07-19", false)]  // turns 18 tomorrow
    [InlineData("2008-07-18", "2026-07-19", true)]   // turned 18 yesterday
    [InlineData("2010-07-19", "2026-07-19", false)]  // exactly 16 years old
    [InlineData("2012-07-19", "2026-07-19", false)]  // 14 years old
    [InlineData("2026-07-19", "2026-07-19", false)]  // born today
    [InlineData("1926-07-19", "2026-07-19", true)]   // 100 years old
    [InlineData("2009-07-19", "2026-07-19", false)]  // exactly 17 years old
    public void IsAtLeast18_ReturnsExpectedEligibility(string birthDateText, string todayText, bool expectedEligible)
    {
        var birthDate = DateTime.Parse(birthDateText);
        var today = DateTime.Parse(todayText);

        var isEligible = AgeCalculator.IsAtLeast18(birthDate, today);

        Assert.Equal(expectedEligible, isEligible);
    }

    // Acceptance-level (BDD / A-TDD) tests, tied directly to the CreateProfilePage
    // acceptance criteria: "You must be at least 18 years old" to create a profile.

    [Fact]
    public void Given_UserTurnedEighteenToday_When_CheckingProfileEligibility_Then_TheyCanCreateAProfile()
    {
        // Given a user whose 18th birthday is today
        var birthDate = new DateTime(2008, 7, 19);
        var today = new DateTime(2026, 7, 19);

        // When checking whether they're eligible to create a profile
        var isEligible = AgeCalculator.IsAtLeast18(birthDate, today);

        // Then they are allowed to proceed
        Assert.True(isEligible, "A user who turns 18 today should be allowed to create a profile.");
    }

    [Fact]
    public void Given_UserTurnsEighteenTomorrow_When_CheckingProfileEligibility_Then_TheyCannotCreateAProfileYet()
    {
        // Given a user who won't turn 18 until tomorrow
        var birthDate = new DateTime(2008, 7, 20);
        var today = new DateTime(2026, 7, 19);

        // When checking whether they're eligible to create a profile
        var isEligible = AgeCalculator.IsAtLeast18(birthDate, today);

        // Then they are blocked from proceeding, matching CreateProfilePage's
        // "You must be at least 18 years old" validation message
        Assert.False(isEligible, "A user who turns 18 tomorrow should not yet be allowed to create a profile.");
    }
}
