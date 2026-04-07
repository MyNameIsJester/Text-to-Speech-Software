using System.Text.Json;
using TravelTracker.Model;

namespace TravelTracker.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    private readonly string _baseUrl = "http://10.0.2.2:5218/api";
    public ApiService()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        _httpClient = new HttpClient(handler);
    }

    public async Task<List<Language>> GetLanguagesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Languages");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var languages = JsonSerializer.Deserialize<List<Language>>(jsonString, options) ?? new List<Language>();

                foreach (var lang in languages)
                {
                    if (!string.IsNullOrEmpty(lang.FlagIcon))
                    {
                        lang.FlagIcon = lang.FlagIcon.Replace("https://localhost:7246", "http://10.0.2.2:5218")
                                                     .Replace("localhost", "10.0.2.2");
                    }
                }

                return languages;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LỖI GỌI BE NGÔN NGỮ: {ex.Message}");
        }
        return new List<Language>();
    }

    public async Task<List<FoodStall>> GetFoodStallsAsync(string langCode = "vi")
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/FoodStalls?lang={langCode}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var stalls = JsonSerializer.Deserialize<List<FoodStall>>(jsonString, options) ?? new List<FoodStall>();

                foreach (var stall in stalls)
                {
                    if (!string.IsNullOrEmpty(stall.ImageUrl))
                    {
                        stall.ImageUrl = stall.ImageUrl.Replace("https://localhost:7246", "http://10.0.2.2:5218")
                                                       .Replace("localhost", "10.0.2.2");
                    }

                    if (!string.IsNullOrEmpty(stall.AudioUrl))
                    {
                        stall.AudioUrl = stall.AudioUrl.Replace("https://localhost:7246", "http://10.0.2.2:5218")
                                                       .Replace("localhost", "10.0.2.2");
                    }
                }

                return stalls;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LỖI GỌI BE QUÁN ĂN: {ex.Message}");
        }

        return new List<FoodStall>();
    }
}