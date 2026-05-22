using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Termini_Api.Models;
using Termini_Api.TerminiDbContext;

namespace Termini_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SportController : ControllerBase
    {
        public readonly TerminiDBContext _terminiDBContext;

        public SportController(TerminiDBContext terminiDBContext)
        {
            _terminiDBContext = terminiDBContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Sport>>> GetSports()
        {
            var sports = await _terminiDBContext.Sports.Select(s => new
            {
                s.SportId,
                s.SportName
            }).ToListAsync();
            return Ok(sports);
        }
    }
}
