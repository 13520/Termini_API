using System.Collections.Generic;

namespace Termini_Api.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;

        public ICollection<Teren> Tereni { get; set; } = new List<Teren>();
    }
}
