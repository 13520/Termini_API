using System.ComponentModel.DataAnnotations;

namespace Termini_Api.Models
{
    public class User
    {
        [Key]
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
    }
}
