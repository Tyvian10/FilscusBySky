namespace FilscusBySky.Web.Models;

public class GrafiekViewModel
{
    public List<string> Maanden { get; set; } = new();
    public List<decimal> Inkomsten { get; set; } = new();
    public List<decimal> Uitgaven { get; set; } = new();
    public List<decimal> NettoResultaat { get; set; } = new();
    public List<string> Categorieen { get; set; } = new();
    public List<decimal> UitgavenPerCategorie { get; set; } = new();
    public List<decimal> Spaarpercentage { get; set; } = new();

    // Vergelijking deze maand vs vorige maand
    public decimal InkomstenDezeMaand { get; set; }
    public decimal UitgavenDezeMaand { get; set; }
    public decimal InkomstenVorigeMaand { get; set; }
    public decimal UitgavenVorigeMaand { get; set; }

    public List<FilscusBySky.Models.Rekening> Rekeningen { get; set; } = new();
    public int? GeselecteerdeRekeningId { get; set; }
}