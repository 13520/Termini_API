namespace Termini_Api.DTOs
{
    public class    TerminDTO
    {
        public long TerminId { get; set; }
        public long ClientId { get; set; }
        public DateTime TerminOd { get; set; }
        public DateTime TerminDo { get; set; }
        public long TerenId { get; set; }
        public long BeneficiaryId { get; set; }
        public bool IsRated { get; set; }
    }
}
