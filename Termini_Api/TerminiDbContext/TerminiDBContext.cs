using Microsoft.EntityFrameworkCore;
using Termini_Api.Models;

namespace Termini_Api.TerminiDbContext
{
    public class TerminiDBContext : DbContext
    {
        public TerminiDBContext(DbContextOptions<TerminiDBContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<Teren> Terens { get; set; }
        public DbSet<Termin> Termins { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
    }
}
