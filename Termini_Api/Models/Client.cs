using System.Collections.Generic;

namespace Termini_Api.Models
{
    public class Client : User
    {
        // Use UserId from base class; do not declare ClientId
        public ICollection<Teren> Tereni { get; set; } = new List<Teren>();
    }
}
