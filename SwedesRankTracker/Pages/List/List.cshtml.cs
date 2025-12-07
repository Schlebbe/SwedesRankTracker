using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SwedesRankTracker.Models;
using SwedesRankTracker.Services.Temple;

namespace SwedesRankTracker.Pages.List
{
    public class ListModel : PageModel
    {
        private readonly ClanDbContext _db;
        private readonly ITempleService _templeService;
        public List<Member>? Members { get; private set; }
        public List<Rank>? Ranks { get; private set; }
        public Dictionary<int, string>? RanksById { get; private set; }

        public ListModel(ClanDbContext db, ITempleService templeService)
        {
            _db = db;
            _templeService = templeService;
        }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Members = await _db.Members
                               .OrderBy(m => m.LastUpdated)
                               .AsNoTracking()
                               .ToListAsync(cancellationToken);

            Ranks = await _db.Ranks
                             .OrderBy(r => r.RankId)
                             .AsNoTracking()
                             .ToListAsync(cancellationToken);

            RanksById = Ranks?.ToDictionary(r => r.RankId, r => r.Name);
        }

        public async Task<IActionResult> OnPostSyncMembersAsync(CancellationToken cancellationToken)
        {
            await _templeService.UpdateAllMembersAsync(force: true);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateMemberAsync(string? username, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest();

            // Retrieve fresh data from the Temple API
            var updatedMember = await _templeService.GetMemberDataAsync(username);
            if (updatedMember == null)
                return NotFound();

            // Persist changes to DB (update existing)
            var existing = await _db.Members.SingleOrDefaultAsync(m => m.UserName == username, cancellationToken);
            if (existing != null)
            {
                existing.TotalLevel = updatedMember.TotalLevel;
                existing.Ehb = updatedMember.Ehb;
                existing.Ehp = updatedMember.Ehp;
                existing.Pets = updatedMember.Pets;
                existing.Collections = updatedMember.Collections;
                existing.RankId = updatedMember.RankId;
                existing.LastUpdated = DateTime.UtcNow;
                _db.Members.Update(existing);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return RedirectToPage();
        }
    }
}
