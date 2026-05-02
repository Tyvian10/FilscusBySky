using FilscusBySky.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FilscusBySky.BusinessLogic;

public class OllamaAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _ollamaUrl;

    public OllamaAIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _ollamaUrl = "/api/generate";
    }

    public async Task<string> GenereerAdviesAsync(Rekening rekening, List<Transactie> transacties)
    {
        var totalInkomen = transacties
            .Where(t => t.Type == TransactieType.Inkomen)
            .Sum(t => t.Bedrag);

        var totalUitgaven = transacties
            .Where(t => t.Type == TransactieType.Uitgave)
            .Sum(t => t.Bedrag);

        // Vorige maand data
        var vorigeMaand = DateTime.UtcNow.AddMonths(-1);
        var totalInkomenVorig = transacties
            .Where(t => t.Type == TransactieType.Inkomen
                && t.Datum.Month == vorigeMaand.Month
                && t.Datum.Year == vorigeMaand.Year)
            .Sum(t => t.Bedrag);

        var totalUitgavenVorig = transacties
            .Where(t => t.Type == TransactieType.Uitgave
                && t.Datum.Month == vorigeMaand.Month
                && t.Datum.Year == vorigeMaand.Year)
            .Sum(t => t.Bedrag);

        // Huidige maand data
        var huidigeMaand = DateTime.UtcNow;
        var totalInkomenHuidig = transacties
            .Where(t => t.Type == TransactieType.Inkomen
                && t.Datum.Month == huidigeMaand.Month
                && t.Datum.Year == huidigeMaand.Year)
            .Sum(t => t.Bedrag);

        var totalUitgavenHuidig = transacties
            .Where(t => t.Type == TransactieType.Uitgave
                && t.Datum.Month == huidigeMaand.Month
                && t.Datum.Year == huidigeMaand.Year)
            .Sum(t => t.Bedrag);

        // Categorie analyse
        var categorieAnalyse = transacties
            .Where(t => t.Type == TransactieType.Uitgave)
            .GroupBy(t => t.Categorie)
            .Select(g => new {
                Categorie = g.Key,
                Totaal = g.Sum(t => t.Bedrag),
                Aantal = g.Count(),
                Percentage = totalUitgaven == 0 ? 0 :
                    Math.Round(g.Sum(t => t.Bedrag) / totalUitgaven * 100, 1)
            })
            .OrderByDescending(c => c.Totaal)
            .ToList();

        var categorieText = string.Join("\n", categorieAnalyse
            .Select(c => $"- {c.Categorie}: EUR {c.Totaal:F2} ({c.Percentage}% van uitgaven, {c.Aantal} transacties)"));

        // Groei berekenen
        var groeiInkomen = totalInkomenVorig == 0 ? 0 :
            Math.Round((totalInkomenHuidig - totalInkomenVorig) / totalInkomenVorig * 100, 1);
        var groeiUitgaven = totalUitgavenVorig == 0 ? 0 :
            Math.Round((totalUitgavenHuidig - totalUitgavenVorig) / totalUitgavenVorig * 100, 1);

        var spaarpercentage = totalInkomen == 0 ? 0 :
            Math.Round((totalInkomen - totalUitgaven) / totalInkomen * 100, 1);

        var prompt = "Je bent een neutrale financieel adviseur. Analyseer dit budget in het Nederlands "
            + "en geef een gestructureerd advies met de volgende 5 onderdelen:\n\n"
            + "=== FINANCIEEL OVERZICHT ===\n"
            + $"Rekening: {rekening.Naam}\n"
            + $"Huidig saldo: EUR {rekening.Saldo:F2}\n"
            + $"Spaarpercentage: {spaarpercentage}%\n\n"
            + "=== INKOMSTENSTROOM ===\n"
            + $"Totaal inkomen (alle tijd): EUR {totalInkomen:F2}\n"
            + $"Inkomen deze maand: EUR {totalInkomenHuidig:F2}\n"
            + $"Inkomen vorige maand: EUR {totalInkomenVorig:F2}\n"
            + $"Groei inkomen: {groeiInkomen}%\n\n"
            + "=== VASTE LASTEN EN UITGAVEN ===\n"
            + $"Totaal uitgaven (alle tijd): EUR {totalUitgaven:F2}\n"
            + $"Uitgaven deze maand: EUR {totalUitgavenHuidig:F2}\n"
            + $"Uitgaven vorige maand: EUR {totalUitgavenVorig:F2}\n"
            + $"Groei uitgaven: {groeiUitgaven}%\n\n"
            + "=== ANALYSE PER CATEGORIE ===\n"
            + categorieText + "\n\n"
            + "=== GEVRAAGDE ANALYSE ===\n"
            + "Schrijf een professioneel financieel adviesrapport in het Nederlands met de volgende 5 alineas.\n"
            + "Gebruik voor elke alinea een duidelijke titel in hoofdletters, gevolgd door een witregel en een volledige paragraaf tekst.\n"
            + "Schrijf in volzinnen, professioneel maar begrijpelijk. Geen opsommingstekens.\n\n"
            + "1. INKOMSTENSTROOM\n"
            + "Analyseer de stabiliteit van de inkomsten. Zijn ze stabiel of schommelend? "
            + "Wat betekent dit voor het budget?\n\n"
            + "2. VASTE LASTEN EN CONTRACTBEHEER\n"
            + "Analyseer het percentage vaste lasten ten opzichte van het inkomen. "
            + "Welke categorieen nemen het grootste deel in? Is er ruimte voor optimalisatie?\n\n"
            + "3. VARIABELE UITGAVEN EN LEEFSTIJL\n"
            + "Bespreek de beinvloedbare uitgaven. Waar zitten de grootste kosten? "
            + "Vergelijk met gangbare normen en identificeer mogelijke besparingen.\n\n"
            + "4. VERGELIJKING MET VORIGE MAAND\n"
            + "Vergelijk de inkomsten en uitgaven van deze maand met vorige maand. "
            + $"Inkomen groeide met {groeiInkomen}% en uitgaven met {groeiUitgaven}%. "
            + "Wat betekent deze trend voor de financiele gezondheid?\n\n"
            + "5. AANBEVELINGEN\n"
            + "Geef 3 concrete, haalbare en persoonlijke aanbevelingen gebaseerd op de bovenstaande analyse. "
            + "Schrijf dit als een vloeiende paragraaf, niet als lijst.\n\n"
            + "Sluit af met een korte samenvattende conclusie van 2 zinnen.";

        try
        {
            var requestBody = new
            {
                model = "llama3.2",
                prompt = prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_ollamaUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"HTTP {response.StatusCode}: {error}";
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            return result.GetProperty("response").GetString()
                ?? FallbackAdvies(rekening, totalInkomen, totalUitgaven);
        }
        catch (Exception ex)
        {
            return $"AI fout: {ex.Message} | URL: {_ollamaUrl} | BaseAddress: {_httpClient.BaseAddress}";
        }
    }

    private string FallbackAdvies(Rekening rekening, decimal inkomen, decimal uitgaven)
    {
        var resultaat = inkomen - uitgaven;
        return $"Rekening '{rekening.Naam}': "
            + $"Inkomen EUR {inkomen:F2}, Uitgaven EUR {uitgaven:F2}. "
            + $"Resultaat: {(resultaat >= 0 ? "+" : "")}EUR {resultaat:F2}. "
            + "(AI momenteel niet beschikbaar.)";
    }
}