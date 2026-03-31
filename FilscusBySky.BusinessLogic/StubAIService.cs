using FilscusBySky.Models;

namespace FilscusBySky.BusinessLogic;

public class StubAIService : IAIService
{
    public Task<string> GenereerAdviesAsync(Rekening rekening, List<Transactie> transacties)
    {
        var totalInkomen = transacties
            .Where(t => t.Type == TransactieType.Inkomen)
            .Sum(t => t.Bedrag);

        var totalUitgaven = transacties
            .Where(t => t.Type == TransactieType.Uitgave)
            .Sum(t => t.Bedrag);

        var resultaat = totalInkomen - totalUitgaven;

        var advies = $"""
            Financieel overzicht voor rekening '{rekening.Naam}':

            Uw totale inkomsten bedragen €{totalInkomen:F2} en uw uitgaven €{totalUitgaven:F2}.
            Het resultaat is {(resultaat >= 0 ? "positief" : "negatief")}: €{resultaat:F2}.

            {(resultaat >= 0
                ? "Goed gedaan! U geeft minder uit dan u verdient. Overweeg het overschot te sparen."
                : "Let op: uw uitgaven overschrijden uw inkomsten. Bekijk welke categorieën kunnen worden verminderd.")}

            Tip: Controleer regelmatig uw transacties om uw budget onder controle te houden.
            """;

        return Task.FromResult(advies);
    }
}