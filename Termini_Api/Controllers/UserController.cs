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
    public class UserController : ControllerBase
    {
        public readonly TerminiDBContext _terminiDBContext;

        public UserController(TerminiDBContext terminiDBContext)
        {
            _terminiDBContext = terminiDBContext;
        }

        [HttpPost("createBeneficiary")]
        public Task<ActionResult> CreateBeneficiary([FromBody]Beneficiary beneficiary)
        {
            try
            {
                if (beneficiary == null)
                {
                    return Task.FromResult<ActionResult>(BadRequest("Beneficiary data is null."));
                }
                else
                {
                    _terminiDBContext.Beneficiaries.Add(beneficiary);
                    _terminiDBContext.SaveChanges();
                }

                return Task.FromResult<ActionResult>(Ok(beneficiary));
            }
            catch (Exception ex)
            {
                return Task.FromResult<ActionResult>(BadRequest(ex.Message));
            }
        }

        [HttpPost("createClient")]
        public Task<ActionResult> CreateClient([FromBody]Client client)
        {
            try
            {
                if (client == null)
                {
                    return Task.FromResult<ActionResult>(BadRequest("Beneficiary data is null."));
                }
                else
                {
                    _terminiDBContext.Clients.Add(client);
                    _terminiDBContext.SaveChanges();
                }

                return Task.FromResult<ActionResult>(Ok(client));
            }
            catch (Exception ex)
            {
                return Task.FromResult<ActionResult>(BadRequest(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<User>>> Login([FromBody] UserDTO user)
        {
            try
            {
                User? userLogin = new User();

                if (user.UserEmail == null && user.UserName != null)
                {
                    userLogin = await _terminiDBContext.Users.Where(u => u.UserName == user.UserName && u.Password == user.Password).FirstOrDefaultAsync();
                }
                else if (user.UserEmail != null && user.UserName == null)
                {
                    userLogin = await _terminiDBContext.Users.Where(u => u.UserEmail == user.UserEmail && u.Password == user.Password).FirstOrDefaultAsync();
                }
                else 
                { 
                    return BadRequest("Please provide either UserName or UserEmail for login.");
                }

                if (userLogin == null)
                {
                    return BadRequest("Invalid User Name or Password. User not found.");
                }
                else
                {
                    return Ok(userLogin);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
