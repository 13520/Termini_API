using System.Buffers.Text;

namespace Termini_Api.Models
{
    public class Teren
    {
        public long TerenId { get; set; }
        public string TerenName { get; set; }
        public DateTime OpenFrom { get; set; } = DateTime.Now;
        public DateTime OpenTo { get; set; } = DateTime.Now;
        public string ImageBase64 { get; set; }
        public City City { get; set; }
        public Sport Sport { get; set; }
        public Client Client { get; set; }

        public ICollection<Termin> Termins { get; set; }

    }
}
