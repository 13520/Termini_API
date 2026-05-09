namespace Termini_Api.Models
{
    public class Client:User
    {
        public long ClientId { get; set; }

        public ICollection<Teren> Tereni { get; set; }
    }
}
