using Microsoft.EntityFrameworkCore;
using Outty.Api.Data;

namespace Outty.Api.Services;

public record CandidateProfile(
    int ProfileId,
    string DisplayName,
    string City,
    string State,
    int SharedInterestCount);

public record SwipeResult(bool IsMatch);

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

        var alreadySwipedProfileIds = await db.Swipes
            .Where(s => s.SwiperProfileId == profileId)
            .Select(s => s.TargetProfileId)
            .ToListAsync();

        var candidates = await db.Profiles
            .Include(p => p.ProfileInterests)
            .Where(p =>
                p.Id != profileId &&
                p.State == requester.State &&
                !alreadySwipedProfileIds.Contains(p.Id))
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

    /// <summary>
    /// Records a like/pass from one profile toward another. If it's a like and the
    /// target had already liked the swiper back, this is a mutual match.
    /// </summary>
    public async Task<SwipeResult?> RecordSwipeAsync(int swiperProfileId, int targetProfileId, bool liked)
    {
        if (swiperProfileId == targetProfileId)
        {
            return null;
        }

        var swiperExists = await db.Profiles.AnyAsync(p => p.Id == swiperProfileId);
        var targetExists = await db.Profiles.AnyAsync(p => p.Id == targetProfileId);

        if (!swiperExists || !targetExists)
        {
            return null;
        }

        var existingSwipe = await db.Swipes.FirstOrDefaultAsync(s =>
            s.SwiperProfileId == swiperProfileId && s.TargetProfileId == targetProfileId);

        if (existingSwipe is null)
        {
            db.Swipes.Add(new Swipe
            {
                SwiperProfileId = swiperProfileId,
                TargetProfileId = targetProfileId,
                Liked = liked,
                CreatedAtUtc = DateTime.UtcNow
            });
        }
        else
        {
            existingSwipe.Liked = liked;
        }

        await db.SaveChangesAsync();

        var isMatch = liked && await db.Swipes.AnyAsync(s =>
            s.SwiperProfileId == targetProfileId &&
            s.TargetProfileId == swiperProfileId &&
            s.Liked);

        return new SwipeResult(isMatch);
    }
}
