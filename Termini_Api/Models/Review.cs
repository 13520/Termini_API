namespace Termini_Api.Models
{
    public class Review
    {
        public long ReviewId { get; set; }
        public string Comment { get; set; }
        public int Grade { get; set; }

        // Foreign keys
        public long BeneficiaryId { get; set; }
        public Beneficiary Beneficiary { get; set; }

        public long TerenId { get; set; }
        public Teren Teren { get; set; }
    }

}
