using Microsoft.EntityFrameworkCore;
using Outty.Api.Data;

namespace Outty.Api.Services;

public record CandidateProfile(
    int ProfileId,
    string DisplayName,
    string City,
    string State,
    int SharedInterestCount);

public class MatchingService(OuttyDbContext db)
{
    public async Task<List<CandidateProfile>> GetCandidatesAsync(int profileId)
    {
        var requester = await db.Profiles
            .Include(p => p.ProfileInterests)
            .FirstOrDefaultAsync(p => p.Id == profileId);

        if (requester is null)
        {
            return [];
        }

        var requesterInterestIds = requester.ProfileInterests
            .Select(pi => pi.InterestId)
            .ToHashSet();

        var candidates = await db.Profiles
            .Include(p => p.ProfileInterests)
            .Where(p => p.Id != profileId && p.State == requester.State)
            .ToListAsync();

        return candidates
            .Select(c => new CandidateProfile(
                c.Id,
                c.DisplayName,
                c.City,
                c.State,
                c.ProfileInterests.Count(pi => requesterInterestIds.Contains(pi.InterestId))))
            .Where(c => c.SharedInterestCount > 0)
            .OrderByDescending(c => c.SharedInterestCount)
            .ThenBy(c => c.DisplayName)
            .ToList();
    }
}
