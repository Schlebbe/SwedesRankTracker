using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SwedesRankTracker.Models;

namespace SwedesRankTracker.Pages.List
{
    public class ListModel : PageModel
    {
        private readonly ClanDbContext _db;
        public List<Member>? Members { get; private set; }

        public ListModel(ClanDbContext db) => _db = db;

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Members = await _db.Members
                               .OrderByDescending(m => m.LastUpdated)
                               .AsNoTracking()
                               .ToListAsync(cancellationToken);
        }
    }
}
