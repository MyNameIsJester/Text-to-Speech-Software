using System.Net.Http.Json;
using AudioGuideAdmin.ViewModels.FoodStalls;
using AudioGuideAPI.DTOs;
using AudioGuideAPI.Models;

namespace AudioGuideAdmin.Services
{
    public class AdminFoodStallApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AdminFoodStallApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        private string ApiBaseUrl =>
            _configuration["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException("Missing ApiSettings:BaseUrl");

        public async Task<List<FoodStall>> GetFoodStallsAsync()
        {
            var url = $"{ApiBaseUrl}/api/FoodStalls/admin";
            var result = await _httpClient.GetFromJsonAsync<List<FoodStallAdminDto>>(url);
            return result?.Select(MapFoodStallModel).ToList() ?? new List<FoodStall>();
        }

        public async Task<FoodStall?> GetFoodStallByIdAsync(int id)
        {
            var url = $"{ApiBaseUrl}/api/FoodStalls/admin/{id}";
            var result = await _httpClient.GetFromJsonAsync<FoodStallAdminDto>(url);
            return result == null ? null : MapFoodStallModel(result);
        }

        public async Task<FoodStall> CreateFoodStallAsync(FoodStall model)
        {
            var dto = MapUpsertDto(model);
            var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/api/FoodStalls/admin", dto);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<FoodStallAdminDto>()
                ?? throw new InvalidOperationException("Create food stall response is empty.");

            return MapFoodStallModel(result);
        }

        public async Task UpdateFoodStallAsync(int id, FoodStall model)
        {
            var dto = MapUpsertDto(model);
            var response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/api/FoodStalls/admin/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteFoodStallAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/api/FoodStalls/admin/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<FoodStallTranslationsEditViewModel?> GetTranslationsEditorAsync(int id)
        {
            var url = $"{ApiBaseUrl}/api/FoodStalls/admin/{id}/translations-editor";
            var dto = await _httpClient.GetFromJsonAsync<FoodStallTranslationsEditorDto>(url);

            if (dto == null) return null;

            return new FoodStallTranslationsEditViewModel
            {
                FoodStallId = dto.FoodStallId,
                StallAddress = dto.StallAddress,
                Vietnamese = new TranslationInputViewModel
                {
                    TranslationId = dto.Vietnamese.TranslationId,
                    LanguageId = dto.Vietnamese.LanguageId,
                    LanguageCode = dto.Vietnamese.LanguageCode,
                    DisplayName = dto.Vietnamese.DisplayName,
                    Name = dto.Vietnamese.Name,
                    Description = dto.Vietnamese.Description,
                    Specialty = dto.Vietnamese.Specialty,
                    AudioUrl = dto.Vietnamese.AudioUrl
                },
                English = new TranslationInputViewModel
                {
                    TranslationId = dto.English.TranslationId,
                    LanguageId = dto.English.LanguageId,
                    LanguageCode = dto.English.LanguageCode,
                    DisplayName = dto.English.DisplayName,
                    Name = dto.English.Name,
                    Description = dto.English.Description,
                    Specialty = dto.English.Specialty,
                    AudioUrl = dto.English.AudioUrl
                }
            };
        }

        public async Task SaveTranslationsAsync(int id, FoodStallTranslationsEditViewModel model)
        {
            var dto = new FoodStallTranslationsEditorDto
            {
                FoodStallId = model.FoodStallId,
                StallAddress = model.StallAddress,
                Vietnamese = new FoodStallTranslationInputDto
                {
                    TranslationId = model.Vietnamese.TranslationId,
                    LanguageId = model.Vietnamese.LanguageId,
                    LanguageCode = model.Vietnamese.LanguageCode,
                    DisplayName = model.Vietnamese.DisplayName,
                    Name = model.Vietnamese.Name ?? "",
                    Description = model.Vietnamese.Description,
                    Specialty = model.Vietnamese.Specialty,
                    AudioUrl = model.Vietnamese.AudioUrl
                },
                English = new FoodStallTranslationInputDto
                {
                    TranslationId = model.English.TranslationId,
                    LanguageId = model.English.LanguageId,
                    LanguageCode = model.English.LanguageCode,
                    DisplayName = model.English.DisplayName,
                    Name = model.English.Name ?? "",
                    Description = model.English.Description,
                    Specialty = model.English.Specialty,
                    AudioUrl = model.English.AudioUrl
                }
            };

            var response = await _httpClient.PutAsJsonAsync(
                $"{ApiBaseUrl}/api/FoodStalls/admin/{id}/translations",
                dto);

            response.EnsureSuccessStatusCode();
        }

        private static FoodStallAdminUpsertDto MapUpsertDto(FoodStall model)
        {
            return new FoodStallAdminUpsertDto
            {
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Radius = model.Radius,
                Priority = model.Priority,
                ImageUrl = model.ImageUrl,
                Address = model.Address,
                PriceRange = model.PriceRange,
                MapLink = model.MapLink,
                IsActive = model.IsActive,
                OwnerUserId = model.OwnerUserId
            };
        }

        private static FoodStall MapFoodStallModel(FoodStallAdminDto dto)
        {
            return new FoodStall
            {
                Id = dto.Id,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Radius = dto.Radius,
                Priority = dto.Priority,
                ImageUrl = dto.ImageUrl,
                Address = dto.Address,
                PriceRange = dto.PriceRange,
                MapLink = dto.MapLink,
                IsActive = dto.IsActive,
                OwnerUserId = dto.OwnerUserId,
                Translations = dto.Translations.Select(t => new FoodStallTranslation
                {
                    Id = t.TranslationId ?? 0,
                    FoodStallId = dto.Id,
                    LanguageId = t.LanguageId,
                    Name = t.Name,
                    Description = t.Description,
                    Specialty = t.Specialty,
                    AudioUrl = t.AudioUrl,
                    Language = new Language
                    {
                        Id = t.LanguageId,
                        LanguageCode = t.LanguageCode,
                        DisplayName = t.DisplayName
                    }
                }).ToList()
            };
        }
    }
}