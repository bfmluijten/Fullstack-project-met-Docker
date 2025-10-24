using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Database;

public class HospitalDbContext : DbContext
{
    public DbSet<Patient> Patients { get; set; }

    public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>().HasData(
            new Patient { Id = 1, Naam = "Bart Luijten", Adres = "Achterdijk 46 C, Odijk", Geboortejaar = 1962 }
        );
        modelBuilder.Entity<Patient>()
            .HasIndex(c => new { c.Naam, c.Geboortejaar })
            .IsUnique();
    }
}
