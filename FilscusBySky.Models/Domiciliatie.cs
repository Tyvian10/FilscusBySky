using System.ComponentModel.DataAnnotations;

namespace FilscusBySky.Models;

public class Domiciliatie
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Naam { get; set; } = string.Empty;

    public decimal Bedrag { get; set; }

    [MaxLength(100)]
    public string Categorie { get; set; } = "Overig";

    public int Dag { get; set; } = 1;

    public bool IsActief { get; set; } = true;

    public int RekeningId { get; set; }
    public Rekening? Rekening { get; set; }
}