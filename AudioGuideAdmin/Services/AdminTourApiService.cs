using System.Net.Http.Json;
using AudioGuideAdmin.ViewModels.Tours;
using AudioGuideAPI.DTOs;
using AudioGuideAPI.Models;

namespace AudioGuideAdmin.Services
{
    public class AdminTourApiService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public AdminTourApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        private string BaseUrl =>
            _config["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException("Missing ApiSettings:BaseUrl");

        // ===== READ =====
        public async Task<List<Tour>> GetToursAsync()
        {
            var url = $"{BaseUrl}/api/Tours/admin";
            var dtos = await _http.GetFromJsonAsync<List<TourAdminDto>>(url) ?? new();

            return dtos.Select(MapToModel).ToList();
        }

        public async Task<Tour?> GetTourByIdAsync(int id)
        {
            var url = $"{BaseUrl}/api/Tours/admin";
            var dtos = await _http.GetFromJsonAsync<List<TourAdminDto>>(url);
            var dto = dtos?.FirstOrDefault(x => x.Id == id);
            return dto == null ? null : MapToModel(dto);
        }

        // ===== CREATE =====
        public async Task<int> CreateAsync(TourEditViewModel vm)
        {
            var dto = MapToDto(vm);

            var res = await _http.PostAsJsonAsync($"{BaseUrl}/api/Tours/admin", dto);
            res.EnsureSuccessStatusCode();

            return await res.Content.ReadFromJsonAsync<int>();
        }

        // ===== UPDATE =====
        public async Task UpdateAsync(int id, TourEditViewModel vm)
        {
            var dto = MapToDto(vm);

            var res = await _http.PutAsJsonAsync($"{BaseUrl}/api/Tours/admin/{id}", dto);
            res.EnsureSuccessStatusCode();
        }

        // ===== DELETE =====
        public async Task DeleteAsync(int id)
        {
            var res = await _http.DeleteAsync($"{BaseUrl}/api/Tours/admin/{id}");
            res.EnsureSuccessStatusCode();
        }

        // ===== MAPPING =====
        private static TourAdminDto MapToDto(TourEditViewModel vm)
        {
            return new TourAdminDto
            {
                Id = vm.Id,
                IsActive = vm.IsActive,

                VietnameseName = vm.Vietnamese.Name,
                VietnameseDescription = vm.Vietnamese.Description,

                EnglishName = vm.English.Name,
                EnglishDescription = vm.English.Description,

                Items = vm.Items
                    .Where(x => x.FoodStallId > 0)
                    .OrderBy(x => x.OrderIndex)
                    .Select(x => new TourItemDto
                    {
                        FoodStallId = x.FoodStallId,
                        OrderIndex = x.OrderIndex
                    }).ToList()
            };
        }

        private static Tour MapToModel(TourAdminDto dto)
        {
            return new Tour
            {
                Id = dto.Id,
                IsActive = dto.IsActive,
                Translations = new List<TourTranslation>
                {
                    new TourTranslation
                    {
                        Language = new Language { LanguageCode = "vi" },
                        Name = dto.VietnameseName,
                        Description = dto.VietnameseDescription
                    },
                    new TourTranslation
                    {
                        Language = new Language { LanguageCode = "en" },
                        Name = dto.EnglishName,
                        Description = dto.EnglishDescription
                    }
                },
                TourItems = dto.Items.Select(x => new TourItem
                {
                    FoodStallId = x.FoodStallId,
                    OrderIndex = x.OrderIndex
                }).ToList()
            };
        }
    }
}