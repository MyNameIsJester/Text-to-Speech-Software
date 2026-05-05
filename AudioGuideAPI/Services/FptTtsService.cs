using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AudioGuideAPI.Configuration;
using Microsoft.Extensions.Options;

namespace AudioGuideAPI.Services
{
    public class FptTtsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FptTtsOptions _options;

        public FptTtsService(
            IHttpClientFactory httpClientFactory,
            IOptions<FptTtsOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public async Task<byte[]> SynthesizeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                throw new InvalidOperationException("Chưa cấu hình FptTts:ApiKey.");
            }

            var normalizedText = NormalizeText(text);

            if (normalizedText.Length < 3)
            {
                throw new ArgumentException("FPT TTS yêu cầu nội dung tối thiểu 3 ký tự.");
            }

            if (normalizedText.Length > 5000)
            {
                throw new ArgumentException("FPT TTS giới hạn tối đa 5000 ký tự mỗi request.");
            }

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_options.BaseUrl);

            using var request = new HttpRequestMessage(HttpMethod.Post, _options.Endpoint)
            {
                Content = new StringContent(normalizedText, Encoding.UTF8, "text/plain")
            };

            request.Headers.Add("api_key", _options.ApiKey);
            request.Headers.Add("voice", _options.Voice);
            request.Headers.Add("speed", _options.Speed);
            request.Headers.Add("format", _options.Format);
            request.Headers.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };

            using var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"FPT TTS lỗi: {(int)response.StatusCode} - {responseContent}");
            }

            var audioUrl = ExtractAudioUrl(responseContent);

            if (string.IsNullOrWhiteSpace(audioUrl))
            {
                throw new Exception($"FPT TTS không trả về audio url hợp lệ: {responseContent}");
            }

            return await DownloadGeneratedAudioAsync(audioUrl);
        }

        private async Task<byte[]> DownloadGeneratedAudioAsync(string audioUrl)
        {
            var client = _httpClientFactory.CreateClient();

            Exception? lastError = null;

            for (var attempt = 1; attempt <= _options.MaxPollAttempts; attempt++)
            {
                try
                {
                    using var response = await client.GetAsync(audioUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var bytes = await response.Content.ReadAsByteArrayAsync();

                        if (bytes.Length > 0)
                        {
                            return bytes;
                        }
                    }

                    lastError = new Exception($"Audio chưa sẵn sàng. HTTP {(int)response.StatusCode}");
                }
                catch (Exception ex)
                {
                    lastError = ex;
                }

                await Task.Delay(_options.PollDelayMilliseconds);
            }

            throw new Exception(
                $"Không tải được file audio từ FPT sau {_options.MaxPollAttempts} lần thử.",
                lastError);
        }

        private static string ExtractAudioUrl(string responseContent)
        {
            using var document = JsonDocument.Parse(responseContent);
            var root = document.RootElement;

            if (root.TryGetProperty("async", out var asyncElement))
            {
                var value = asyncElement.GetString();

                if (IsUrl(value))
                {
                    return value!;
                }
            }

            if (root.TryGetProperty("message", out var messageElement))
            {
                var value = messageElement.GetString();

                if (IsUrl(value))
                {
                    return value!;
                }
            }

            if (root.TryGetProperty("data", out var dataElement))
            {
                var value = dataElement.GetString();

                if (IsUrl(value))
                {
                    return value!;
                }
            }

            return "";
        }

        private static bool IsUrl(string? value)
        {
            return !string.IsNullOrWhiteSpace(value)
                   && Uri.TryCreate(value, UriKind.Absolute, out _);
        }

        private static string NormalizeText(string text)
        {
            return text
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ")
                .Trim();
        }
    }
}