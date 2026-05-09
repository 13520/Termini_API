namespace Termini_Api.Models
{
    public class Sport
    {
        public int SportId { get; set; }
        public string SportName { get; set; } = string.Empty;
        public ICollection<Teren> Tereni { get; set; }

    }
}
