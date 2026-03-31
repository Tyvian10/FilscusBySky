namespace FilscusBySky.Web.Models;

public class GrafiekViewModel
{
    public List<string> Maanden { get; set; } = new();
    public List<decimal> Inkomsten { get; set; } = new();
    public List<decimal> Uitgaven { get; set; } = new();
    public List<string> Categorieen { get; set; } = new();
    public List<decimal> UitgavenPerCategorie { get; set; } = new();
}