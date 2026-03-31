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

        var categorieën = transacties
            .Where(t => t.Type == TransactieType.Uitgave)
            .GroupBy(t => t.Categorie)
            .Select(g => $"{g.Key}: EUR {g.Sum(t => t.Bedrag):F2}");

        var prompt = $"Analyseer dit budget en geef neutraal advies in het Nederlands (max 3 paragrafen):\n\n"
            + $"Rekening: {rekening.Naam}\n"
            + $"Saldo: EUR {rekening.Saldo:F2}\n"
            + $"Inkomen: EUR {totalInkomen:F2}\n"
            + $"Uitgaven: EUR {totalUitgaven:F2}\n"
            + $"Categorieen: {string.Join(", ", categorieën)}";

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