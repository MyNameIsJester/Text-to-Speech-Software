namespace AudioGuideAPI.Configuration
{
    public class FptTtsOptions
    {
        public string ApiKey { get; set; } = "";
        public string BaseUrl { get; set; } = "https://api.fpt.ai";
        public string Endpoint { get; set; } = "/hmi/tts/v5";
        public string Voice { get; set; } = "banmai";
        public string Speed { get; set; } = "0";
        public string Format { get; set; } = "mp3";
        public int MaxPollAttempts { get; set; } = 12;
        public int PollDelayMilliseconds { get; set; } = 5000;
    }
}