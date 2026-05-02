using System.Net.Http.Json;
using AudioGuideAPI.Models;

namespace AudioGuideAdmin.Services
{
    public class AdminPlaybackLogApiService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public AdminPlaybackLogApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        private string BaseUrl =>
            _config["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException("Missing ApiSettings:BaseUrl");

        public async Task<List<PlaybackLog>> GetPlaybackLogsAsync()
        {
            var url = $"{BaseUrl}/api/PlaybackLogs/admin";
            var dtos = await _http.GetFromJsonAsync<List<PlaybackLogAdminApiDto>>(url) ?? new();

            return dtos.Select(MapToModel).ToList();
        }

        private static PlaybackLog MapToModel(PlaybackLogAdminApiDto dto)
        {
            return new PlaybackLog
            {
                Id = dto.Id,
                FoodStallId = dto.FoodStallId,
                LanguageCode = dto.LanguageCode,
                TriggerType = dto.TriggerType,
                Status = dto.Status,
                StartedAt = dto.StartedAt,
                EndedAt = dto.EndedAt,
                ActualDurationSeconds = dto.ActualDurationSeconds,
                FoodStall = new FoodStall
                {
                    Id = dto.FoodStallId,
                    Address = dto.FoodStallAddress,
                    Translations = new List<FoodStallTranslation>
                    {
                        new FoodStallTranslation
                        {
                            Language = new Language { LanguageCode = "vi" },
                            Name = dto.VietnameseName
                        },
                        new FoodStallTranslation
                        {
                            Language = new Language { LanguageCode = "en" },
                            Name = dto.EnglishName
                        }
                    }
                }
            };
        }

        private class PlaybackLogAdminApiDto
        {
            public int Id { get; set; }
            public int FoodStallId { get; set; }
            public string FoodStallAddress { get; set; } = "";
            public string VietnameseName { get; set; } = "";
            public string EnglishName { get; set; } = "";
            public string LanguageCode { get; set; } = "";
            public string TriggerType { get; set; } = "";
            public string Status { get; set; } = "";
            public DateTime StartedAt { get; set; }
            public DateTime? EndedAt { get; set; }
            public int ActualDurationSeconds { get; set; }
        }
    }
}