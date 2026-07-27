using System.Net.Http.Json;

namespace Outty.Mobile.Services;

public record GoogleLoginResult(int UserId, string Email, bool HasProfile);

public record InterestSelection(string Name, string ExperienceLevel);

public record CreateProfileRequest(
    int UserId,
    string DisplayName,
    DateOnly BirthDate,
    string City,
    string State,
    string ZipCode,
    string? Pronouns,
    string? Bio,
    string PreferredDistance,
    int SearchRadiusMiles,
    List<InterestSelection> Interests,
    List<string> Goals);

public record CreateProfileResult(int Id);

public record CandidateProfile(
    int ProfileId,
    string DisplayName,
    string City,
    string State,
    int SharedInterestCount);

public record SwipeResult(bool IsMatch, int? ConversationId);

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiOptions.BaseAddress)
        };
    }

    public async Task<GoogleLoginResult> LoginWithGoogleAsync(string idToken)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "/users/login",
            new { IdToken = idToken });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GoogleLoginResult>()
            ?? throw new InvalidOperationException("Login response was empty.");
    }

    public async Task<CreateProfileResult> CreateProfileAsync(CreateProfileRequest request)
    {
        using var response = await _httpClient.PostAsJsonAsync("/profiles", request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Unable to create profile ({(int)response.StatusCode}): {errorBody}");
        }

        return await response.Content.ReadFromJsonAsync<CreateProfileResult>()
            ?? throw new InvalidOperationException("Create profile response was empty.");
    }

    public async Task<List<CandidateProfile>> GetCandidatesAsync(int profileId)
    {
        using var response = await _httpClient.GetAsync($"/matches/candidates/{profileId}");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<CandidateProfile>>()
            ?? [];
    }

    public async Task<SwipeResult> RecordSwipeAsync(int swiperProfileId, int targetProfileId, bool liked)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "/matches/swipe",
            new { SwiperProfileId = swiperProfileId, TargetProfileId = targetProfileId, Liked = liked });

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Unable to record swipe ({(int)response.StatusCode}): {errorBody}");
        }

        return await response.Content.ReadFromJsonAsync<SwipeResult>()
            ?? throw new InvalidOperationException("Swipe response was empty.");
    }
}
