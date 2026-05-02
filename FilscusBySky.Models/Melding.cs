namespace FilscusBySky.Models;

public class Melding
{
    public int Id { get; set; }
    public string Bericht { get; set; } = string.Empty;
    public bool IsGelezen { get; set; } = false;
    public DateTime AangemaaktOp { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
}