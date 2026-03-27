using System.ComponentModel.DataAnnotations;

namespace FilscusBySky.Models;

public enum TransactieType { Inkomen, Uitgave }

public class Transactie
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Beschrijving { get; set; } = string.Empty;

    public decimal Bedrag { get; set; }

    public TransactieType Type { get; set; } = TransactieType.Uitgave;

    [MaxLength(100)]
    public string Categorie { get; set; } = "Overig";

    public DateTime Datum { get; set; } = DateTime.UtcNow;

    public int RekeningId { get; set; }
    public Rekening? Rekening { get; set; }
}