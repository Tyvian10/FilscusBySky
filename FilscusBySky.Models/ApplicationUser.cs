using Microsoft.AspNetCore.Identity;

namespace FilscusBySky.Models;

public class ApplicationUser : IdentityUser
{
    public string VolledigeNaam { get; set; } = string.Empty;
    public bool IsVisueleBeperking { get; set; } = false;
    public DateTime AangemeldOp { get; set; } = DateTime.UtcNow;

    public ICollection<Rekening> Rekeningen { get; set; } = new List<Rekening>();
}