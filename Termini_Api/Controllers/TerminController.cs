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
    public class TerminController : ControllerBase
    {
        public readonly TerminiDBContext _terminiDBContext;

        public TerminController(TerminiDBContext terminiDBContext)
        {
            _terminiDBContext = terminiDBContext;
        }

        [HttpPost]
        public async Task<ActionResult> CreateTermin([FromBody] TerminDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Termin data is null.");

                var teren = await _terminiDBContext.Terens.FindAsync(dto.TerenId);
                var beneficiary = await _terminiDBContext.Beneficiaries.FindAsync(dto.BeneficiaryId);

                if (teren == null || beneficiary == null)
                    return BadRequest("Invalid TerenId or BeneficiaryId.");

                var termin = new Termin
                {
                    TerminOd = dto.TerminOd,
                    TerminDo = dto.TerminDo,
                    Teren = teren,
                    Beneficiary = beneficiary
                };

                bool exists = _terminiDBContext.Termins.Any(t =>
                    t.TerminOd == termin.TerminOd &&
                    t.TerminDo == termin.TerminDo &&
                    t.Teren.TerenId == dto.TerenId);

                if (exists)
                    return BadRequest("Termin already exists.");

                await _terminiDBContext.Termins.AddAsync(termin);
                await _terminiDBContext.SaveChangesAsync();

                return Ok("Termin created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("OldTermins")]
        public async Task<ActionResult<List<Termin>>> GetOldTermins()
        {
            try
            {
                var termins = await _terminiDBContext.Termins.Where(t => t.TerminOd <= DateTime.UtcNow).ToListAsync();
                return Ok(termins);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("NewTermins")]
        public async Task<ActionResult<List<Termin>>> GetNewTermins()
        {
            try
            {
                var termins = await _terminiDBContext.Termins.Where(t => t.TerminOd >= DateTime.UtcNow).ToListAsync();
                return Ok(termins);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
