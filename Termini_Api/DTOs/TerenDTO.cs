using Termini_Api.Models;

namespace Termini_Api.DTOs
{
    public class TerenDTO
    {
        public long TerenId { get; set; }
        public string TerenName { get; set; }
        public string ImageBase64 { get; set; }
        public TimeSpan OpenFrom { get; set; } 
        public TimeSpan OpenTo { get; set; }
        public int CityId { get; set; }
        public int SportId { get; set; }
        public long ClientId { get; set; }
    }
}
