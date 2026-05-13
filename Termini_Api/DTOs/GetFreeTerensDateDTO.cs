namespace Termini_Api.DTOs
{
    public class GetFreeTerensDateDTO
    {
        public DateTime TerminOd { get; set; }
        public DateTime TerminDo { get; set; }
        public int CityId { get; set; }
        public int SportId { get; set; }
    }
}
