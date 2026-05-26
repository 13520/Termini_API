namespace Termini_Api.DTOs
{
    public class ReviewDTO
    {
        public string Comment { get; set; }
        public int Grade { get; set; }
        public long BeneficiaryId { get; set; }
        public long TerenId { get; set; }
    }
}
