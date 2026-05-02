using Microsoft.AspNetCore.Identity;

namespace FilscusBySky.Models;

public class ApplicationUser : IdentityUser
{
    public string VolledigeNaam { get; set; } = string.Empty;
    public bool IsVisueleBeperking { get; set; } = false;
    public DateTime AangemeldOp { get; set; } = DateTime.UtcNow;
    public int MeldingDag { get; set; } = 1; // 1 = Maandag, 2 = Dinsdag, ...7 = Zondag
    public TimeSpan MeldingTijd { get; set; } = new TimeSpan(9, 0, 0); // Standaard 09:00
    public ICollection<Rekening> Rekeningen { get; set; } = new List<Rekening>();
    public ICollection<Melding> Meldingen { get; set; } = new List<Melding>();
    public ICollection<Categorie> Categorieen { get; set; } = new List<Categorie>();
}