using System.Net.Http.Json;
using AudioGuideAdmin.ViewModels.Dashboard;

namespace AudioGuideAdmin.Services
{
    public class WebVisitApiService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public WebVisitApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        private string BaseUrl =>
            _config["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException("Missing ApiSettings:BaseUrl");

        public async Task<WebVisitStatsViewModel> GetStatsAsync()
        {
            var url = $"{BaseUrl}/api/WebVisits/stats";

            var result = await _http.GetFromJsonAsync<WebVisitStatsViewModel>(url);

            return result ?? new WebVisitStatsViewModel();
        }
    }
}