using SwedesRankTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwedesRankTracker.Services.Temple
{
    public interface ITempleService
    {
        /// <summary>
        /// Returns the usernames of clan members from the external temple API.
        /// </summary>
        Task<List<string>> GetMemberUsernamesAsync();
        Task<Member> GetMemberDataAsync(string username);
    }
}