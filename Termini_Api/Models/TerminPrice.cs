namespace Termini_Api.Models
{
    public class TerminPrice
    {
        public long TerminPriceId { get; set; }
        public decimal Price { get; set; }
        public long? TerenId { get; set; }
        public Teren? Teren { get; set; }
    }
}
