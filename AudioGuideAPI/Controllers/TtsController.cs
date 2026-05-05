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
        private readonly FptTtsService _fptTtsService;

        public TtsController(
            ElevenLabsTtsService elevenLabsTtsService,
            FptTtsService fptTtsService)
        {
            _elevenLabsTtsService = elevenLabsTtsService;
            _fptTtsService = fptTtsService;
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

                if (normalizedLanguageCode == "vi")
                {
                    audioBytes = await _fptTtsService.SynthesizeAsync(request.Text);
                }
                else if (normalizedLanguageCode == "en")
                {
                    audioBytes = await _elevenLabsTtsService.SynthesizeAsync(
                        request.Text,
                        normalizedLanguageCode);
                }
                else
                {
                    return BadRequest($"LanguageCode không hỗ trợ: '{request.LanguageCode}'.");
                }

                var fileName = $"tts-{normalizedLanguageCode}-{Guid.NewGuid():N}.mp3";
                return File(audioBytes, "audio/mpeg", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "TTS generation failed.",
                    provider = normalizedLanguageCode == "vi" ? "FPT.AI" : "ElevenLabs",
                    languageCode = normalizedLanguageCode,
                    detail = ex.Message
                });
            }
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                message = "TTS controller is ready.",
                viProvider = "FPT.AI",
                enProvider = "ElevenLabs"
            });
        }

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
                    message = "Không lấy được danh sách voices ElevenLabs.",
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
                    message = "Không lấy được danh sách models ElevenLabs.",
                    detail = ex.Message
                });
            }
        }

        private static string NormalizeLanguageCode(string? languageCode)
        {
            return languageCode?.Trim().ToLowerInvariant() switch
            {
                "en" => "en",
                "en-us" => "en",
                "en-gb" => "en",
                "vi" => "vi",
                "vi-vn" => "vi",
                _ => "vi"
            };
        }
    }
}