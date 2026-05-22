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
    public class CityController : ControllerBase
    {
        public readonly TerminiDBContext _terminiDBContext;

        public CityController(TerminiDBContext terminiDBContext)
        {
            _terminiDBContext = terminiDBContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<City>>> GetCities()
        {
            var city = await _terminiDBContext.Cities.Select(s => new
            {
                s.CityId,
                s.CityName
            }).ToListAsync();
            return Ok(city);
        }
    }
}
