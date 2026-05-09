using Termini_Api.Models;

namespace Termini_Api.DTOs
{
    public class TerenDTO
    {
        public long TerenId { get; set; }
        public string TerenName { get; set; }
        public string ImageBase64 { get; set; }
        public DateTime OpenFrom { get; set; } = DateTime.Now;
        public DateTime OpenTo { get; set; } = DateTime.Now;
        public int CityId { get; set; }
        public int SportId { get; set; }
        public long ClientId { get; set; }
    }
}
