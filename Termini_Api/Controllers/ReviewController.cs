using Humanizer;
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
    public class ReviewController : ControllerBase
    {
        public readonly TerminiDBContext _terminiDBContext;

        public ReviewController(TerminiDBContext terminiDbContext)
        {
            _terminiDBContext = terminiDbContext;
        }

        [HttpGet("{terenId}")]
        public async Task<ActionResult> GetReviews(long terenId)
        {
            try
            {
                var reviews = await _terminiDBContext.Reviews.Where(r => r.TerenId == terenId).ToListAsync();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateReview([FromBody]ReviewDTO dto)
        {
            try
            {
                var review = new Review
                {
                    Comment = dto.Comment,
                    Grade = dto.Grade,
                    BeneficiaryId = dto.BeneficiaryId,
                    TerenId = dto.TerenId
                };

                _terminiDBContext.Reviews.Add(review);
                await _terminiDBContext.SaveChangesAsync();

                return Ok(review);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
