using System.Net.Http.Json;
using FilscusBySky.Models;

namespace FilscusBySky.MAUI.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Rekening>> GetRekeningenAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<Rekening>>("api/RekeningApi");
            return result ?? new List<Rekening>();
        }
        catch
        {
            return new List<Rekening>();
        }
    }

    public async Task<List<Transactie>> GetTransactiesAsync(int rekeningId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<Transactie>>($"api/RekeningApi/{rekeningId}/transacties");
            return result ?? new List<Transactie>();
        }
        catch
        {
            return new List<Transactie>();
        }
    }
}