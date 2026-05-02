using System.Text.Json;
using AudioGuideWeb.Models;

namespace AudioGuideWeb.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _apiBaseUrl;

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            _apiBaseUrl = configuration["ApiSettings:BaseUrl"]
                ?? throw new InvalidOperationException("Missing ApiSettings:BaseUrl configuration.");
        }

        public async Task<List<LanguageViewModel>> GetLanguagesAsync()
        {
            var url = $"{_apiBaseUrl}/api/Languages";
            using var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<List<LanguageViewModel>>(stream, _jsonOptions);

            return result ?? new List<LanguageViewModel>();
        }

        public async Task<List<FoodStallViewModel>> GetFoodStallsAsync(string lang = "vi")
        {
            var url = $"{_apiBaseUrl}/api/FoodStalls?lang={Uri.EscapeDataString(lang)}";
            using var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<List<FoodStallViewModel>>(stream, _jsonOptions);

            return result ?? new List<FoodStallViewModel>();
        }

        public async Task<FoodStallViewModel?> GetFoodStallByIdAsync(int id, string lang = "vi")
        {
            var foodStalls = await GetFoodStallsAsync(lang);
            return foodStalls.FirstOrDefault(x => x.Id == id);
        }

        public async Task<List<TourViewModel>> GetToursAsync(string lang = "vi")
        {
            var url = $"{_apiBaseUrl}/api/Tours?lang={Uri.EscapeDataString(lang)}";
            using var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<List<TourViewModel>>(stream, _jsonOptions);

            return result ?? new List<TourViewModel>();
        }

        public async Task<TourViewModel?> GetTourByIdAsync(int id, string lang = "vi")
        {
            var tours = await GetToursAsync(lang);
            return tours.FirstOrDefault(x => x.Id == id);
        }
    }
}