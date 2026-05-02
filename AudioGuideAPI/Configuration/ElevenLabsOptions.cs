namespace AudioGuideAPI.Configuration
{
    public class ElevenLabsOptions
    {
        public string ApiKey { get; set; } = "";
        public string BaseUrl { get; set; } = "https://api.elevenlabs.io";
        public string VoiceIdVi { get; set; } = "";
        public string VoiceIdEn { get; set; } = "";
        public string ModelId { get; set; } = "eleven_multilingual_v2";
    }
}