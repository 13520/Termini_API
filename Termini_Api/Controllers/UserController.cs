using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

                return Task.FromResult<ActionResult>(Ok("User has been created"));
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

                return Task.FromResult<ActionResult>(Ok("User has been created"));
            }
            catch (Exception ex)
            {
                return Task.FromResult<ActionResult>(BadRequest(ex.Message));
            }
        }
    }
}
