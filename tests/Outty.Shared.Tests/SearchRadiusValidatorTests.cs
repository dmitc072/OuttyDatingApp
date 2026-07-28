using Outty.Shared.Utilities;

namespace Outty.Shared.Tests;

public class SearchRadiusValidatorTests
{
    [Theory]
    [InlineData(4, false)]   // just below minimum
    [InlineData(5, true)]    // minimum, inclusive
    [InlineData(6, true)]
    [InlineData(25, true)]   // default value
    [InlineData(50, true)]
    [InlineData(99, true)]
    [InlineData(100, true)]  // maximum, inclusive
    [InlineData(101, false)] // just above maximum
    [InlineData(0, false)]
    [InlineData(-5, false)]
    public void IsValid_ReturnsExpectedResult(int miles, bool expectedValid)
    {
        var isValid = SearchRadiusValidator.IsValid(miles);

        Assert.Equal(expectedValid, isValid);
    }

    [Theory]
    [InlineData(40, 40)]    // valid value passes through unchanged
    [InlineData(3, 5)]      // below minimum, clamps up to minimum
    [InlineData(150, 100)]  // above maximum, clamps down to maximum
    [InlineData(5, 5)]      // exactly the minimum boundary
    [InlineData(100, 100)]  // exactly the maximum boundary
    public void Normalize_WithRequestedValue_ClampsToValidRange(int requested, int expected)
    {
        var normalized = SearchRadiusValidator.Normalize(requested);

        Assert.Equal(expected, normalized);
    }

    [Fact]
    public void Normalize_WithNullRequest_ReturnsDefault()
    {
        var normalized = SearchRadiusValidator.Normalize(null);

        Assert.Equal(SearchRadiusValidator.DefaultMiles, normalized);
    }

    // Acceptance-level (BDD / A-TDD) test, tied to US-08's acceptance criteria:
    // "search radius slider (5-100 miles), defaulting to 25 miles if not set"

    [Fact]
    public void Given_UserHasNotSetASearchRadius_When_ProfileIsLoaded_Then_RadiusDefaultsToTwentyFiveMiles()
    {
        // Given a user who has never set a search radius preference
        int? requestedMiles = null;

        // When their effective search radius is resolved
        var effectiveRadius = SearchRadiusValidator.Normalize(requestedMiles);

        // Then it defaults to 25 miles, matching the US-08 acceptance criteria
        Assert.Equal(25, effectiveRadius);
    }
}
