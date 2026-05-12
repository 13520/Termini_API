using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Termini_Api.DTOs;
using Termini_Api.Models;
using Termini_Api.TerminiDbContext;

namespace Termini_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerenController : ControllerBase
    {
        public readonly TerminiDBContext _terminiDBContext;

        public TerenController(TerminiDBContext terminiDBContext)
        {
            _terminiDBContext = terminiDBContext;
        }

        [HttpPost]
        public async Task<ActionResult> CreateTeren([FromBody] List<TerenDTO> terens)
        {
            try
            {
                if (terens == null || !terens.Any())
                    return BadRequest("Teren data is null or empty.");

                // Mapiranje DTO → domain model
                var terenEntities = new List<Teren>();

                foreach (var dto in terens)
                {
                    var city = await _terminiDBContext.Cities.FindAsync(dto.CityId);
                    var sport = await _terminiDBContext.Sports.FindAsync(dto.SportId);
                    var client = await _terminiDBContext.Clients.FindAsync(dto.ClientId);

                    if (city == null || sport == null || client == null)
                        return BadRequest("Invalid CityId, SportId or ClientId.");

                    var teren = new Teren
                    {
                        TerenName = dto.TerenName,
                        OpenFrom = dto.OpenFrom,
                        OpenTo = dto.OpenTo,
                        ImageBase64 = dto.ImageBase64,
                        CityId = dto.CityId,
                        SportId = dto.SportId,
                        ClientId = dto.ClientId
                    };

                    terenEntities.Add(teren);
                }

                await _terminiDBContext.Terens.AddRangeAsync(terenEntities);
                await _terminiDBContext.SaveChangesAsync();

                return Ok("Teren(s) created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetFreeTerens")]
        public async Task<ActionResult> GetFreeTerens([FromBody] GetFreeTerensDateDTO dateDTO)
        {
            try
            {
                var terens = await _terminiDBContext.Termins
                                                    .Where(t => t.TerminDo <= dateDTO.TerminOd || t.TerminOd >= dateDTO.TerminDo)
                                                    .Select(t => t.Teren)
                                                    .ToListAsync();
                var uniqueTerens = terens.Where(t => t.CityId == dateDTO.CityId).ToList();

                return Ok(uniqueTerens);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("TerensByClient/{clientId}")]
        public async Task<ActionResult> GetTerensByClient(int clientId)
        {
            try
            {
                var terens = await _terminiDBContext.Terens
                                                    .Where(t => t.Client.UserId == clientId)
                                                    .ToListAsync();
                return Ok(terens);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
