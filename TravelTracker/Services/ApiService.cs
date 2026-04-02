using System.Text.Json;
using TravelTracker.Model;

namespace TravelTracker.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    private readonly string _baseUrl = "https://10.0.2.2:7246/api";

    public ApiService()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        _httpClient = new HttpClient(handler);
    }

    public async Task<List<Poi>> GetPoisAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Poi");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Poi>>(jsonString, options);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LỖI GỌI BE: {ex.Message}");
        }

        return new List<Poi>();
    }
}