using Microsoft.EntityFrameworkCore;
using SwedesRankTracker.Models;
using SwedesRankTracker.Services.Temple;

namespace SwedesRankTracker.Services.Startup
{
    public class StartupInitializer : IStartupInitializer
    {
        private readonly ClanDbContext _db;
        private readonly ITempleService _templeService;

        public StartupInitializer(ClanDbContext db, ITempleService templeService)
        {
            _db = db;
            _templeService = templeService;
        }

        public async Task RunAsync()
        {
            var usernames = await _templeService.GetClanMemberUsernamesAsync();

            if (usernames is null || !usernames.Any())
                return;

            foreach (var username in usernames)
            {
                // Only add if not present (compare by UserName)
                var exists = await _db.Members.AnyAsync(m => m.UserName == username);
                if (!exists)
                {
                    var member = await _templeService.GetMemberDataAsync(username);

                    _db.Members.Add(member);
                }
            }
            
            await _db.SaveChangesAsync();
        }
    }
}