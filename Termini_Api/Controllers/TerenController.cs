using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Termini_Api.DTOs;
using Termini_Api.Models;
using Termini_Api.TerminiDbContext;

namespace Termini_Api.Controllers
{
    [Authorize]
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
                var terminPriceEntities = new List<TerminPrice>();

                // Skupi sve ID-jeve unapred
                var cityIds   = terens.Select(t => t.CityId).Distinct();
                var sportIds  = terens.Select(t => t.SportId).Distinct();
                var clientIds = terens.Select(t => t.ClientId).Distinct();

                // Napuni hash tabele
                var cities = await _terminiDBContext.Cities
                    .Where(c => cityIds.Contains(c.CityId))
                    .ToDictionaryAsync(c => c.CityId);

                var sports = await _terminiDBContext.Sports
                    .Where(s => sportIds.Contains(s.SportId))
                    .ToDictionaryAsync(s => s.SportId);

                var clients = await _terminiDBContext.Clients
                    .Where(cl => clientIds.Contains(cl.UserId))
                    .ToDictionaryAsync(cl => cl.UserId);

                // Mapiranje preko LINQ-a
                terenEntities = terens.Select(dto =>
                {
                    if (!cities.TryGetValue(dto.CityId, out var city) ||
                        !sports.TryGetValue(dto.SportId, out var sport) ||
                        !clients.TryGetValue(dto.ClientId, out var client))
                    {
                        throw new ArgumentException("Invalid CityId, SportId or ClientId.");
                    }

                    return new Teren
                    {
                        TerenName = dto.TerenName,
                        OpenFrom = dto.OpenFrom,
                        OpenTo = dto.OpenTo,
                        ImageBase64 = dto.ImageBase64,
                        CityId = dto.CityId,
                        SportId = dto.SportId,
                        ClientId = dto.ClientId,
                        Address = dto.Address,
                        IsClosed = false
                    };
                }).ToList();

                terminPriceEntities = terens.Where(dto => dto.PricePerHour.HasValue)
                                            .Select(dto =>
                                            {
                                                var teren = terenEntities.First(t =>
                                                    t.CityId == dto.CityId &&
                                                    t.SportId == dto.SportId &&
                                                    t.ClientId == dto.ClientId &&
                                                    t.TerenName == dto.TerenName);
                                            
                                                return new TerminPrice
                                                {
                                                    Price = dto.PricePerHour.Value,
                                                    Teren = teren
                                                };
                                            })
                                            .ToList();


                await _terminiDBContext.Terens.AddRangeAsync(terenEntities);
                await _terminiDBContext.TerminPrices.AddRangeAsync(terminPriceEntities);
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

                var allTerens = await _terminiDBContext.Terens
                    .Where(t => t.CityId == dateDTO.CityId && t.SportId == dateDTO.SportId)
                    .ToListAsync();


                var busyTerens = await _terminiDBContext.Termins
                    .Where(t => !(t.TerminDo <= dateDTO.TerminOd || t.TerminOd >= dateDTO.TerminDo))
                    .Select(t => t.TerenId)
                    .Distinct()
                    .ToListAsync();


                //var freeTerens = allTerens
                //    .Where(t => !busyTerens.Contains(t.TerenId) && t.IsClosed == false)
                //    .ToList();
                var freeTerens = allTerens
                                 .Where(t => !busyTerens.Contains(t.TerenId) && t.IsClosed == false && t.IsDeleted == false)
                                 .Select(t => new
                                 {
                                     t.TerenId,
                                     t.TerenName,
                                     t.CityId,
                                     t.SportId,
                                     t.ClientId,
                                     t.OpenFrom,
                                     t.OpenTo,
                                     t.Address,
                                     t.ImageBase64,
                                     AverageGrade = Math.Round(_terminiDBContext.Reviews
                                                                                        .Where(r => r.TerenId == t.TerenId)
                                                                                        .Select(r => (double?)r.Grade)
                                                                                        .Average() ?? 0, 2
                                                                                 )
                                 })
                                 .ToList();


                return Ok(freeTerens);
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
                                                    .Where(t => t.ClientId == clientId && t.IsClosed == false && t.IsDeleted == false)
                                                    .Select(t => new
                                                    {
                                                        t.TerenId,
                                                        t.TerenName,
                                                        t.CityId,
                                                        t.SportId,
                                                        t.ClientId,
                                                        t.OpenFrom,
                                                        t.OpenTo,
                                                        t.Address,
                                                        t.ImageBase64,
                                                        AverageGrade = Math.Round(_terminiDBContext.Reviews
                                                                                        .Where(r => r.TerenId == t.TerenId)
                                                                                        .Select(r => (double?)r.Grade)
                                                                                        .Average() ?? 0, 2
                                                                                 )
                                                    })
                                                    .ToListAsync();
                return Ok(terens);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{terenId}")]
        public async Task<ActionResult> GetTerensById(int terenId)
        {
            try
            {
                var terens = await _terminiDBContext.Terens
                                                    .Where(t => t.TerenId == terenId && t.IsClosed == false && t.IsDeleted == false)
                                                    .Select(t => new
                                                    {
                                                        t.TerenId,
                                                        t.TerenName,
                                                        t.CityId,
                                                        t.SportId,
                                                        t.ClientId,
                                                        t.OpenFrom,
                                                        t.OpenTo,
                                                        t.Address,
                                                        t.ImageBase64,
                                                        AverageGrade = Math.Round(_terminiDBContext.Reviews
                                                                                        .Where(r => r.TerenId == t.TerenId)
                                                                                        .Select(r => (double?)r.Grade)
                                                                                        .Average() ?? 0, 2
                                                                                 )
                                                    })
                                                    .FirstAsync();
                return Ok(terens);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateTeren([FromBody] TerenDTO terenDTO)
        {
            try
            {
                var existingTeren = await _terminiDBContext.Terens.FindAsync(terenDTO.TerenId);
                if (existingTeren == null)
                    return NotFound("Teren not found.");
                existingTeren.TerenName = terenDTO.TerenName;
                existingTeren.OpenFrom = terenDTO.OpenFrom;
                existingTeren.OpenTo = terenDTO.OpenTo;
                existingTeren.ImageBase64 = terenDTO.ImageBase64;
                existingTeren.CityId = terenDTO.CityId;
                existingTeren.SportId = terenDTO.SportId;
                existingTeren.ClientId = terenDTO.ClientId;
                existingTeren.Address = terenDTO.Address;
                existingTeren.IsClosed = terenDTO.IsClosed;
                _terminiDBContext.Terens.Update(existingTeren);
                await _terminiDBContext.SaveChangesAsync();
                return Ok("Teren updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{terenId}")]
        public async Task<ActionResult> DeleteTeren(long terenId)
        {
            try
            {
                var teren = await _terminiDBContext.Terens.FindAsync(terenId);
                if (teren == null)
                    return NotFound("Teren not found.");
                teren.IsDeleted = true;

                await _terminiDBContext.SaveChangesAsync();
                return Ok("Teren deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
