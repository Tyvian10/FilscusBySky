using System.ComponentModel.DataAnnotations;

namespace FilscusBySky.Models;

public class Categorie
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Naam { get; set; } = string.Empty;

    public bool IsStandaard { get; set; } = false;

    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}