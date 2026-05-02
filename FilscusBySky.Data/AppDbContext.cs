using FilscusBySky.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FilscusBySky.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Rekening> Rekeningen => Set<Rekening>();
    public DbSet<Transactie> Transacties => Set<Transactie>();
    public DbSet<Melding> Meldingen => Set<Melding>();
    public DbSet<Domiciliatie> Domiciliaties => Set<Domiciliatie>();
    public DbSet<Categorie> Categorieen => Set<Categorie>();
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

        builder.Entity<Domiciliatie>()
            .Property(d => d.Bedrag)
            .HasPrecision(18, 2);

        builder.Entity<Categorie>()
    .HasOne(c => c.User)
    .WithMany(u => u.Categorieen)
    .HasForeignKey(c => c.UserId)
    .OnDelete(DeleteBehavior.Cascade)
    .IsRequired(false);
    }
}