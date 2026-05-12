using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Termini_Api.Models
{
    public class Termin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TerminId { get; set; }
        public DateTime TerminOd { get; set; }
        public DateTime TerminDo { get; set; }

        public long? TerenId { get; set; }
        public Teren? Teren { get; set; }

        public long? BeneficiaryId { get; set; }
        public Beneficiary? Beneficiary { get; set; }
    }
}
