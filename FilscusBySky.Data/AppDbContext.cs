using FilscusBySky.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FilscusBySky.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Rekening> Rekeningen => Set<Rekening>();
    public DbSet<Transactie> Transacties => Set<Transactie>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Precisie voor geldbedragen
        builder.Entity<Rekening>()
            .Property(r => r.Saldo)
            .HasPrecision(18, 2);

        builder.Entity<Transactie>()
            .Property(t => t.Bedrag)
            .HasPrecision(18, 2);
    }
}