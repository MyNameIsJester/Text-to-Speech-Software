using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AudioGuideAPI.Configuration;
using Microsoft.Extensions.Options;

namespace AudioGuideAPI.Services
{
    public class ElevenLabsTtsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ElevenLabsOptions _options;

        public ElevenLabsTtsService(
            IHttpClientFactory httpClientFactory,
            IOptions<ElevenLabsOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public async Task<byte[]> SynthesizeAsync(string text, string languageCode)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text không được để trống.");
            }

            var voiceId = ResolveVoiceId(languageCode);

            if (string.IsNullOrWhiteSpace(voiceId))
            {
                throw new InvalidOperationException($"Chưa cấu hình VoiceId cho ngôn ngữ '{languageCode}'.");
            }

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(_options.BaseUrl);
            client.DefaultRequestHeaders.Add("xi-api-key", _options.ApiKey);

            var requestBody = new
            {
                text = text,
                model_id = _options.ModelId,
                voice_settings = new
                {
                    stability = 0.5,
                    similarity_boost = 0.75
                }
            };

            var json = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/v1/text-to-speech/{voiceId}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("audio/mpeg"));

            using var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"ElevenLabs TTS lỗi: {(int)response.StatusCode} - {errorContent}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }

        private string ResolveVoiceId(string languageCode)
        {
            return languageCode?.ToLower() switch
            {
                "vi" => _options.VoiceIdVi,
                "en" => _options.VoiceIdEn,
                _ => _options.VoiceIdVi
            };
        }

        public async Task<string> GetVoicesRawAsync()
        {
            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(_options.BaseUrl);
            client.DefaultRequestHeaders.Add("xi-api-key", _options.ApiKey);

            using var response = await client.GetAsync("/v2/voices");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"ElevenLabs voices error: {(int)response.StatusCode} - {content}");
            }

            return content;
        }

        public async Task<string> GetModelsRawAsync()
        {
            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(_options.BaseUrl);
            client.DefaultRequestHeaders.Add("xi-api-key", _options.ApiKey);

            using var response = await client.GetAsync("/v1/models");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"ElevenLabs models error: {(int)response.StatusCode} - {content}");
            }

            return content;
        }
    }
}