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
    public class NotificationController : ControllerBase
    {
        public readonly TerminiDBContext _terminiDBContext;

        public NotificationController(TerminiDBContext terminiDBContext)
        {
            _terminiDBContext = terminiDBContext;
        }

        [HttpGet("NotificationByClient/{clientId}")]
        public async Task<ActionResult> GetNotificationsById(int clientId)
        {
            try
            {
                var terens = await _terminiDBContext.Notifications
                                                    .Where(t => t.clientId == clientId)
                                                    .Select(t => new
                                                    {
                                                        t.clientId,
                                                        t.message,
                                                        t.isRead
                                                    
                                                    })
                                                    .ToListAsync();
                return Ok(terens);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<ActionResult> CreateNotification([FromBody] NotificationDTO notificationDto)
        {
            try
            {
                var notification = new Notification
                {
                    message = notificationDto.message,
                    clientId = notificationDto.clientId,
                    isRead = false,
                    created_at = DateTime.UtcNow
                };

                _terminiDBContext.Notifications.Add(notification);
                await _terminiDBContext.SaveChangesAsync();

                return Ok(notification);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    innerException = ex.InnerException?.Message,
                    innerInnerException = ex.InnerException?.InnerException?.Message
                });
            }
        }

    }
}
