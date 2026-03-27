using System.ComponentModel.DataAnnotations;

namespace FilscusBySky.Models;

public class Rekening
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Naam { get; set; } = string.Empty;

    public decimal Saldo { get; set; } = 0;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public ICollection<Transactie> Transacties { get; set; } = new List<Transactie>();
}