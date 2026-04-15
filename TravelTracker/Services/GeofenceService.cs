using Microsoft.Maui.Devices.Sensors;
using TravelTracker.Model;

namespace TravelTracker.Services;

public class GeofenceService
{
    private HashSet<int> _playedStallIds = new HashSet<int>();
    private CancellationTokenSource _ttsCancellationTokenSource;

    public async Task CheckAndTriggerAudioAsync(Location userLocation, List<FoodStall> stalls, string currentLang, ApiService apiService)
    {
        if (userLocation == null || stalls == null) return;

        var targetStall = stalls
            .Where(s => !_playedStallIds.Contains(s.Id))
            .Select(s => new { Stall = s, Distance = Location.CalculateDistance(userLocation, new Location(s.Latitude, s.Longitude), DistanceUnits.Kilometers) * 1000 })
            .Where(x => x.Distance <= x.Stall.Radius)
            .OrderByDescending(x => x.Stall.Priority)
            .Select(x => x.Stall)
            .FirstOrDefault();

        if (targetStall != null)
        {
            _playedStallIds.Add(targetStall.Id);

            CancelCurrentAudio();

            await TriggerTTSAsync(targetStall, currentLang, apiService);
        }
    }

    private async Task TriggerTTSAsync(FoodStall stall, string langCode, ApiService apiService)
    {
        _ttsCancellationTokenSource = new CancellationTokenSource();

        try
        {
            string textToRead = stall.Description;
            if (string.IsNullOrWhiteSpace(textToRead)) textToRead = $"Bạn đang ở gần {stall.Name}.";

            int words = textToRead.Split(' ').Length;
            int duration = (words / 3) + 2;

            _ = apiService.SendPlaybackLogAsync(stall.Id, langCode, "GPS", duration);

            await TextToSpeech.Default.SpeakAsync(textToRead, cancelToken: _ttsCancellationTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
            System.Diagnostics.Debug.WriteLine($"[AUDIO INTERRUPTED] Đã cắt ngang audio cũ để phát audio mới.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Lỗi TTS: {ex.Message}");
        }
    }

    public void CancelCurrentAudio()
    {
        if (_ttsCancellationTokenSource != null && !_ttsCancellationTokenSource.IsCancellationRequested)
        {
            _ttsCancellationTokenSource.Cancel();
        }
    }

    public void ResetHistory() => _playedStallIds.Clear();
}