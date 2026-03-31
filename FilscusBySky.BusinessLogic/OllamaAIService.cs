using FilscusBySky.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FilscusBySky.BusinessLogic;

public class OllamaAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private const string OllamaUrl = "http://localhost:11434/api/generate";

    public OllamaAIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GenereerAdviesAsync(Rekening rekening, List<Transactie> transacties)
    {
        var totalInkomen = transacties
            .Where(t => t.Type == TransactieType.Inkomen)
            .Sum(t => t.Bedrag);

        var totalUitgaven = transacties
            .Where(t => t.Type == TransactieType.Uitgave)
            .Sum(t => t.Bedrag);

        var categorieën = transacties
            .Where(t => t.Type == TransactieType.Uitgave)
            .GroupBy(t => t.Categorie)
            .Select(g => $"{g.Key}: €{g.Sum(t => t.Bedrag):F2}");

        var prompt = $"""
            Analyseer dit budget en geef neutraal advies in het Nederlands (max 3 paragrafen):

            Rekening: {rekening.Naam}
            Saldo: €{rekening.Saldo:F2}
            Inkomen: €{totalInkomen:F2}
            Uitgaven: €{totalUitgaven:F2}
            Categorieën: {string.Join(", ", categorieën)}
            """;

        try
        {
            var requestBody = new
            {
                model = "tinyllama",
                prompt = prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(OllamaUrl, content);

            if (!response.IsSuccessStatusCode)
                return FallbackAdvies(rekening, totalInkomen, totalUitgaven);

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            return result.GetProperty("response").GetString()
                ?? FallbackAdvies(rekening, totalInkomen, totalUitgaven);
        }
        catch
        {
            // Ollama niet beschikbaar → fallback naar stub
            return FallbackAdvies(rekening, totalInkomen, totalUitgaven);
        }
    }

    private string FallbackAdvies(Rekening rekening, decimal inkomen, decimal uitgaven)
    {
        var resultaat = inkomen - uitgaven;
        return $"Rekening '{rekening.Naam}': " +
               $"Inkomen €{inkomen:F2}, Uitgaven €{uitgaven:F2}. " +
               $"Resultaat: {(resultaat >= 0 ? "+" : "")}€{resultaat:F2}. " +
               $"(AI momenteel niet beschikbaar — installeer Ollama met tinyllama voor volledig advies.)";
    }
}