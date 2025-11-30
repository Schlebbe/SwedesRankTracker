using Microsoft.AspNetCore.Mvc;
using SwedesRankTracker.Services.Temple;

namespace SwedesRankTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TempleController : ControllerBase
    {
        private readonly ITempleService _templeService;

        public TempleController(ITempleService templeService)
        {
            _templeService = templeService;
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok(new { status = "running" });
        }

        [HttpGet("")]
        public async Task<IActionResult> GetTempleInfo()
        {

            return Ok();
        }
    }
}