using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Termini_Api.DTOs;
using Termini_Api.Models;
using Termini_Api.TerminiDbContext;

namespace Termini_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TerminController : ControllerBase
    {
        public readonly TerminiDBContext _terminiDBContext;
        private readonly IChannel _channel;

        public TerminController(TerminiDBContext terminiDBContext, IChannel channel)
        {
            _terminiDBContext = terminiDBContext;
            _channel = channel;
        }

        [HttpPost]
        public async Task<ActionResult> CreateTermin([FromBody] TerminDTO dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                return BadRequest("Termin data is null.");

            // Publish DTO u RabbitMQ
            if (dto.TerminOd <= dto.TerminDo && dto.TerminDo >= dto.TerminOd.AddHours(1))
            {
                var json = JsonSerializer.Serialize(dto);
                var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "termins",
                mandatory: true, 
                body:  body, 
                cancellationToken);

                return Ok("Termin queued successfully.");
            }
            else
            {
                return BadRequest("TerminOd must be before TerminDo.");
            }
        }

        [HttpGet("OldTermins/{id}")]
        public async Task<ActionResult<List<Termin>>> GetOldTermins(long id)
        {
            try
            {
                var termins = await _terminiDBContext.Termins
                                                     .Where(t => t.TerminDo <= DateTime.UtcNow && t.BeneficiaryId == id)
                                                     .Select(t => 
                                                     new { 
                                                        TerminId = t.TerminId,
                                                        
                                                        TerminOd = t.TerminOd,
                                                        TerminDo = t.TerminDo,
                                                        TerenName = _terminiDBContext.Terens.Where(tr => tr.TerenId == t.TerenId).Select(t => t.TerenName).FirstOrDefault(),
                                                        TerenId = t.TerenId,
                                                        BeneficiaryId = t.BeneficiaryId,
                                                        FullPrice = t.FullPrice,
                                                        IsRated = t.IsRated,
                                                     })
                                                     .OrderBy(t => t.TerminOd)
                                                     .ToListAsync();
                return Ok(termins);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("IsRated/{id}")]
        public async Task<ActionResult> MarkTerminAsRated(long id)
        {
            try
            {
                var termin = await _terminiDBContext.Termins.FindAsync(id);
                if (termin == null)
                    return NotFound($"Termin with ID {id} not found.");
                termin.IsRated = true;
                await _terminiDBContext.SaveChangesAsync();
                return Ok("Termin marked as rated.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("NewTermins/{id}")]
        public async Task<ActionResult<List<Termin>>> GetNewTermins(long id)
        {
            try
            {
                var termins = await _terminiDBContext.Termins
                                                     .Where(t => t.TerminDo >= DateTime.UtcNow && t.BeneficiaryId == id)
                                                     .Select(t =>
                                                     new {
                                                         TerminId = t.TerminId,
                                                         TerminOd = t.TerminOd,
                                                         TerminDo = t.TerminDo,
                                                         TerenName = _terminiDBContext.Terens.Where(tr => tr.TerenId == t.TerenId).Select(t => t.TerenName).FirstOrDefault(),
                                                         TerenId = t.TerenId,
                                                         BeneficiaryId = t.BeneficiaryId,
                                                         FullPrice = t.FullPrice,
                                                         IsRated = t.IsRated,
                                                     })
                                                     .OrderBy(t => t.TerminOd)
                                                     .ToListAsync();
                return Ok(termins);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("NewTerminsByClient/{id}")]
        public async Task<ActionResult<List<Termin>>> GetNewTerminsByClient(long id)
        {
            try
            {
                
                var clientTerens = await _terminiDBContext.Terens
                                                          .Where(t => t.ClientId == id)
                                                          .Select(t => (long?)t.TerenId)
                                                          .ToListAsync();

                if (!clientTerens.Any())
                {
                    return NotFound($"Client {id} do not have registered courts.");
                }

                var termins = await _terminiDBContext.Termins
                    .Where(t => clientTerens.Contains(t.TerenId) && t.TerminDo >= DateTime.UtcNow)
                    .ToListAsync();

                return Ok(termins);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("OldTerminsByClient/{id}")]
        public async Task<ActionResult<List<Termin>>> GetOldTerminsByClient(long id)
        {
            try
            {

                var clientTerens = await _terminiDBContext.Terens
                                                          .Where(t => t.ClientId == id)
                                                          .Select(t => (long?)t.TerenId)
                                                          .ToListAsync();

                if (!clientTerens.Any())
                {
                    return NotFound($"Client {id} do not have registered courts.");
                }

                var termins = await _terminiDBContext.Termins
                    .Where(t => clientTerens.Contains(t.TerenId) && t.TerminDo <= DateTime.UtcNow)
                    .ToListAsync();

                return Ok(termins);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //[HttpPost]  LEFT FOR TESTING PURPOSES, TO BE REMOVED IN PRODUCTION
        //public async Task<ActionResult> CreateTermin([FromBody] TerminDTO dto)
        //{
        //    try
        //    {
        //        if (dto == null)
        //            return BadRequest("Termin data is null.");

        //        var teren = await _terminiDBContext.Terens.FindAsync(dto.TerenId);
        //        var beneficiary = await _terminiDBContext.Beneficiaries.FindAsync(dto.BeneficiaryId);

        //        if (teren == null || beneficiary == null)
        //            return BadRequest("Invalid TerenId or BeneficiaryId.");

        //        var termin = new Termin
        //        {
        //            TerminOd = dto.TerminOd,
        //            TerminDo = dto.TerminDo,
        //            Teren = teren,
        //            Beneficiary = beneficiary
        //        };

        //        bool exists = _terminiDBContext.Termins.Any(t =>
        //            t.TerminOd == termin.TerminOd &&
        //            t.TerminDo == termin.TerminDo &&
        //            t.Teren.TerenId == dto.TerenId);

        //        if (exists)
        //            return BadRequest("Termin already exists.");

        //        await _terminiDBContext.Termins.AddAsync(termin);
        //        await _terminiDBContext.SaveChangesAsync();

        //        return Ok("Termin created successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
