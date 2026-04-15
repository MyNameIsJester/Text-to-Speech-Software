using System.Text.Json;
using System.Text;
using TravelTracker.Model;
using System.Linq;
using System.Globalization;

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
                    if (stall.Translations != null && stall.Translations.Count > 0)
                    {
                        var trans = stall.Translations.FirstOrDefault(t => t.LanguageCode == langCode)
                                 ?? stall.Translations.First();

                        stall.Name = trans.Name;
                        stall.Description = trans.Description;
                        stall.Specialty = trans.Specialty;
                        stall.AudioUrl = trans.AudioUrl;
                    }

                    if (!string.IsNullOrEmpty(stall.ImageUrl))
                        stall.ImageUrl = stall.ImageUrl.Replace("https://localhost:7246", "http://10.0.2.2:5218").Replace("localhost", "10.0.2.2");

                    if (!string.IsNullOrEmpty(stall.AudioUrl))
                        stall.AudioUrl = stall.AudioUrl.Replace("https://localhost:7246", "http://10.0.2.2:5218").Replace("localhost", "10.0.2.2");
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

    public async Task SendPlaybackLogAsync(int stallId, string langCode, string triggerType, int durationSeconds)
    {
        try
        {
            var logData = new
            {
                FoodStallId = stallId,
                LanguageCode = langCode,
                TriggerType = triggerType,
                DurationSeconds = durationSeconds
            };

            var json = JsonSerializer.Serialize(logData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Remove("X-Tunnel-Skip-AntiPhishing-Page");
            _httpClient.DefaultRequestHeaders.Add("X-Tunnel-Skip-AntiPhishing-Page", "true");

            var response = await _httpClient.PostAsync($"{_baseUrl}/PlaybackLogs", content);
            if (response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"[LOG SUCCESS] Đã lưu log phát Audio: Quán {stallId} - Loại: {triggerType}");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[LOG ERROR] Lỗi {response.StatusCode}: {error}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LOG ERROR] Lỗi gửi PlaybackLog: {ex.Message}");
        }
    }

    public async Task<List<Tour>> GetToursAsync(string langCode = "vi")
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Tours");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var tours = JsonSerializer.Deserialize<List<Tour>>(jsonString, options) ?? new List<Tour>();

                foreach (var tour in tours)
                {
                    if (tour.Translations != null && tour.Translations.Any())
                    {
                        var trans = tour.Translations.FirstOrDefault(t => t.Language?.LanguageCode == langCode)
                                 ?? tour.Translations.FirstOrDefault(t => t.Language?.LanguageCode == "vi")
                                 ?? tour.Translations.First();

                        tour.Name = trans.Name;
                        tour.Description = trans.Description;
                    }

                    if (tour.TourItems != null && tour.TourItems.Any())
                    {
                        tour.TourItems = tour.TourItems.OrderBy(ti => ti.OrderIndex).ToList();

                        foreach (var item in tour.TourItems)
                        {
                            if (item.FoodStall?.Translations != null && item.FoodStall.Translations.Any())
                            {
                                var stallTrans = item.FoodStall.Translations.FirstOrDefault(t => t.LanguageCode == langCode)
                                              ?? item.FoodStall.Translations.First();

                                item.FoodStall.Name = stallTrans.Name;
                                item.FoodStall.Description = stallTrans.Description;
                            }
                        }
                    }
                }
                return tours;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LỖI GỌI BE TOURS: {ex.Message}");
        }
        return new List<Tour>();
    }
    public async Task<List<Microsoft.Maui.Devices.Sensors.Location>> GetRouteAsync(List<Microsoft.Maui.Devices.Sensors.Location> points)
    {
        try
        {
            if (points == null || points.Count < 2) return new List<Microsoft.Maui.Devices.Sensors.Location>();

            var coords = string.Join(";", points.Select(p =>
                $"{p.Longitude.ToString(CultureInfo.InvariantCulture)},{p.Latitude.ToString(CultureInfo.InvariantCulture)}"));

            string url = $"https://router.project-osrm.org/route/v1/foot/{coords}?geometries=geojson&overview=full";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "TravelTrackerApp/1.0");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var coordinates = doc.RootElement
                    .GetProperty("routes")[0]
                    .GetProperty("geometry")
                    .GetProperty("coordinates");

                var routeLocations = new List<Microsoft.Maui.Devices.Sensors.Location>();
                foreach (var coord in coordinates.EnumerateArray())
                {
                    double lon = coord[0].GetDouble();
                    double lat = coord[1].GetDouble();
                    routeLocations.Add(new Microsoft.Maui.Devices.Sensors.Location(lat, lon));
                }
                return routeLocations;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LỖI TÌM ĐƯỜNG OSRM: {ex.Message}");
        }
        return new List<Microsoft.Maui.Devices.Sensors.Location>();
    }
}
