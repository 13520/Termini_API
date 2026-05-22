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
        private readonly IModel _channel;

        public TerminController(TerminiDBContext terminiDBContext, IModel channel)
        {
            _terminiDBContext = terminiDBContext;
            _channel = channel;
        }

        [HttpPost]
        public async Task<ActionResult> CreateTermin([FromBody] TerminDTO dto)
        {
            if (dto == null)
                return BadRequest("Termin data is null.");

            // Publish DTO u RabbitMQ
            var json = JsonSerializer.Serialize(dto);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange: "",
                                  routingKey: "termins",
                                  basicProperties: null,
                                  body: body);

            return Ok("Termin queued successfully.");
        }

        [HttpGet("OldTermins/{id}")]
        public async Task<ActionResult<List<Termin>>> GetOldTermins(long id)
        {
            try
            {
                var termins = await _terminiDBContext.Termins.Where(t => t.TerminDo <= DateTime.UtcNow && t.BeneficiaryId == id).ToListAsync();
                return Ok(termins);
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
                var termins = await _terminiDBContext.Termins.Where(t => t.TerminDo >= DateTime.UtcNow && t.BeneficiaryId == id).ToListAsync();
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
