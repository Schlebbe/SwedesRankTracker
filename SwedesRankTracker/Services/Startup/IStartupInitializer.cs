using System.Threading.Tasks;

namespace SwedesRankTracker.Services.Startup
{
    public interface IStartupInitializer
    {
        Task RunAsync();
    }
}