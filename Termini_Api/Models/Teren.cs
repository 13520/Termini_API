using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Termini_Api.Models
{
    public class Teren
    {
        [Key]
        public long TerenId { get; set; }
        public string TerenName { get; set; } = string.Empty;
        public TimeSpan OpenFrom { get; set; }
        public TimeSpan OpenTo { get; set; }
        public string? ImageBase64 { get; set; }

        // FKs (optional)
        public int? CityId { get; set; }
        public City? City { get; set; }

        public int? SportId { get; set; }
        public Sport? Sport { get; set; }

        public long? ClientId { get; set; }
        public Client? Client { get; set; }
        public string? Address { get; set; }
        public bool IsClosed { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Termin> Termins { get; set; } = new List<Termin>();
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
    }
}
