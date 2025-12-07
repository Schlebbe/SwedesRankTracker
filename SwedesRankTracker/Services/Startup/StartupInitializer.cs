using Microsoft.EntityFrameworkCore;
using SwedesRankTracker.Exceptions;
using SwedesRankTracker.Models;
using SwedesRankTracker.Services.Temple;

namespace SwedesRankTracker.Services.Startup
{
    public class StartupInitializer : IStartupInitializer
    {
        private readonly ClanDbContext _db;
        private readonly ITempleService _templeService;
        private List<Rank> _ranks;

        public StartupInitializer(ClanDbContext db, ITempleService templeService)
        {
            _db = db;
            _templeService = templeService;
            _ranks = new List<Rank>();
        }

        public async Task RunAsync()
        {
            _ranks = await _db.Ranks.OrderByDescending(r => r.RankId).ToListAsync();
            var usernames = await _templeService.GetMemberUsernamesAsync();

            if (usernames is null || usernames.Count == 0)
                return;

            foreach (var username in usernames) 
            {
                // Only add if not present (compare by UserName)
                var exists = await _db.Members.OrderBy(m => m.LastUpdated).AnyAsync(m => m.UserName == username);
                if (!exists)
                {
                    try
                    {
                        Console.WriteLine($"Updating user: {username} ({usernames.IndexOf(username)}/{usernames.Count})");
                        var member = await _templeService.GetMemberDataAsync(username);

                        CalculateRank(member);
                        _db.Members.Add(member);
                    }
                    catch (TooManyRequestsException)
                    {
                        continue;
                    }
                    catch (NotQualifiedMemberException) //TODO: Not currently implemented and might not be needed
                    {
                        Console.WriteLine("User did not qualify for any ranks");
                        continue;
                    }
                }
            }

            await _db.SaveChangesAsync();
        }

        private void CalculateRank(Member member)
        {
            member.RankId = 1; // Default rank

            foreach (var rank in _ranks)
            {
                if (member.Ehb >= rank.MinEhb ||
                    member.Ehp >= rank.MinEhp ||
                    (rank.MinTotalLevel != null && member.TotalLevel >= rank.MinTotalLevel) ||
                    member.Collections >= rank.MinCollections ||
                    member.Pets >= rank.MinPets)
                {
                    member.RankId = rank.RankId;
                    break;
                }
            }
        }
    }
}