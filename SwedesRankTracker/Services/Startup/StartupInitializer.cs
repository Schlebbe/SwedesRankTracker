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

        public StartupInitializer(ClanDbContext db, ITempleService templeService)
        {
            _db = db;
            _templeService = templeService;
        }

        public async Task RunAsync()
        {
            await _templeService.UpdateAllMembersAsync(force: false);
        }
    }
}