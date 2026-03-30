using FilscusBySky.Models;

namespace FilscusBySky.Web.Models;

public class DashboardViewModel
{
    public List<Rekening> Rekeningen { get; set; } = new();
    public decimal TotaalSaldo { get; set; }
    public decimal TotaalInkomen { get; set; }
    public decimal TotaalUitgaven { get; set; }
    public string GebruikersNaam { get; set; } = string.Empty;

    public decimal Resultaat => TotaalInkomen - TotaalUitgaven;
}