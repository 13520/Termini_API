using System.Text.Json.Serialization;

namespace Termini_Api.Models
{
    public class Beneficiary : User
    {
        // Use UserId from base class; do not declare BeneficiaryId
        [JsonIgnore]
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
    }
}
