using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        private readonly IConfiguration _configuration;

        public UserController(TerminiDBContext terminiDBContext, IConfiguration configuration)
        {
            _terminiDBContext = terminiDBContext;
            _configuration = configuration;
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
        public async Task<ActionResult> Login([FromBody] UserDTO user)
        {
            try
            {
                User? userLogin = null;

                
                userLogin = await _terminiDBContext.Users
                        .FirstOrDefaultAsync(u => u.UserEmail == user.UserEmail && u.Password == user.Password);


                if (userLogin == null)
                {
                    return BadRequest("Invalid User Name or Password. User not found.");
                }

                // Generiši token
                var token = GenerateJwtToken(userLogin);

                return Ok(new { 
                                Token = token,
                                UserId = userLogin.UserId,
                                UserEmail = userLogin.UserEmail,
                                FName = userLogin.FName,
                                LName = userLogin.LName,
                                UserPhone = userLogin.UserPhone
                            }
                         );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.UserEmail ?? ""),
                new Claim("UserId", user.UserId.ToString())
            };
        
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMonths(3),//DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds);
        
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
