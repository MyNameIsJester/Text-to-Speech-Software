using AudioGuideAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AudioGuideAPI.Controllers
{
    public class SynthesizeTtsRequest
    {
        public string Text { get; set; } = "";
        public string LanguageCode { get; set; } = "vi";
    }

    [Route("api/[controller]")]
    [ApiController]
    public class TtsController : ControllerBase
    {
        private readonly ElevenLabsTtsService _elevenLabsTtsService;
        private readonly GoogleTranslateTtsService _googleTranslateTtsService;

        public TtsController(
            ElevenLabsTtsService elevenLabsTtsService,
            GoogleTranslateTtsService googleTranslateTtsService)
        {
            _elevenLabsTtsService = elevenLabsTtsService;
            _googleTranslateTtsService = googleTranslateTtsService;
        }

        [HttpPost("synthesize")]
        public async Task<IActionResult> Synthesize([FromBody] SynthesizeTtsRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("Text không được để trống.");
            }

            var normalizedLanguageCode = NormalizeLanguageCode(request.LanguageCode);

            try
            {
                byte[] audioBytes;

                if (normalizedLanguageCode == "en")
                {
                    audioBytes = await _elevenLabsTtsService.SynthesizeAsync(
                        request.Text,
                        normalizedLanguageCode);
                }
                else if (normalizedLanguageCode == "vi")
                {
                    audioBytes = await _googleTranslateTtsService.SynthesizeAsync(
                        request.Text,
                        normalizedLanguageCode);
                }
                else
                {
                    return BadRequest($"LanguageCode không hỗ trợ: '{request.LanguageCode}'.");
                }

                var fileName = $"tts-{Guid.NewGuid():N}.mp3";
                return File(audioBytes, "audio/mpeg", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "TTS generation failed.",
                    detail = ex.Message
                });
            }
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                message = "TTS controller is ready."
            });
        }

        // Hai endpoint này hiện chỉ phản ánh provider ElevenLabs (phục vụ debug tiếng Anh).
        [HttpGet("voices")]
        public async Task<IActionResult> GetVoices()
        {
            try
            {
                var result = await _elevenLabsTtsService.GetVoicesRawAsync();
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Không lấy được danh sách voices.",
                    detail = ex.Message
                });
            }
        }

        [HttpGet("models")]
        public async Task<IActionResult> GetModels()
        {
            try
            {
                var result = await _elevenLabsTtsService.GetModelsRawAsync();
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Không lấy được danh sách models.",
                    detail = ex.Message
                });
            }
        }

        private static string NormalizeLanguageCode(string? languageCode)
        {
            return languageCode?.Trim().ToLowerInvariant() switch
            {
                "en" => "en",
                "vi" => "vi",
                _ => "vi"
            };
        }
    }
}