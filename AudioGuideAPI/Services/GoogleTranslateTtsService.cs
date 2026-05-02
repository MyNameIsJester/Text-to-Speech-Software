using System.Net;

namespace AudioGuideAPI.Services
{
    public class GoogleTranslateTtsService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleTranslateTtsService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<byte[]> SynthesizeAsync(string text, string languageCode)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text không được để trống.");
            }

            var lang = ResolveLanguageCode(languageCode);

            // Google Translate TTS kiểu này không ổn với text dài.
            // Cắt ngắn để giảm lỗi 400.
            var normalizedText = NormalizeText(text);
            var safeText = TruncateText(normalizedText, 180);

            var client = _httpClientFactory.CreateClient();

            var url =
                "https://translate.google.com/translate_tts" +
                $"?ie=UTF-8&q={WebUtility.UrlEncode(safeText)}" +
                $"&tl={WebUtility.UrlEncode(lang)}" +
                "&client=tw-ob";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");

            using var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Google Translate TTS lỗi: {(int)response.StatusCode} - {errorContent}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }

        private static string ResolveLanguageCode(string? languageCode)
        {
            return languageCode?.Trim().ToLowerInvariant() switch
            {
                "vi" => "vi",
                "en" => "en",
                _ => "vi"
            };
        }

        private static string NormalizeText(string text)
        {
            return text
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ")
                .Trim();
        }

        private static string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            if (text.Length <= maxLength)
            {
                return text;
            }

            var shortened = text[..maxLength];
            var lastSpace = shortened.LastIndexOf(' ');

            if (lastSpace > 0)
            {
                shortened = shortened[..lastSpace];
            }

            return shortened.Trim();
        }
    }
}