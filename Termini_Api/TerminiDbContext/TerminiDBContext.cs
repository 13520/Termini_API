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
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<Teren> Terens { get; set; }
        public DbSet<Termin> Termins { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<TerminPrice> TerminPrices { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Default: TPH (Users table with Discriminator). Keep default unless you want TPT.
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Beneficiary)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BeneficiaryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Teren)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.TerenId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
