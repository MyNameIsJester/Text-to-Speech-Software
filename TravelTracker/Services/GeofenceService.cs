using Microsoft.Maui.Devices.Sensors;
using TravelTracker.Model;

namespace TravelTracker.Services;

public class GeofenceService
{
    private HashSet<int> _playedStallIds = new HashSet<int>();
    private bool _isSpeaking = false;

    public async Task CheckAndTriggerAudioAsync(Location userLocation, List<FoodStall> stalls)
    {
        if (_isSpeaking || userLocation == null || stalls == null) return;

        foreach (var stall in stalls)
        {
            var stallLocation = new Location(stall.Latitude, stall.Longitude);

            double distanceInMeters = Location.CalculateDistance(userLocation, stallLocation, DistanceUnits.Kilometers) * 1000;

            if (distanceInMeters <= stall.Radius && !_playedStallIds.Contains(stall.Id))
            {
                _playedStallIds.Add(stall.Id);

                await TriggerTTSAsync(stall);

                break;
            }
        }
    }

    private async Task TriggerTTSAsync(FoodStall stall)
    {
        _isSpeaking = true;
        try
        {
            string textToRead = $"Bạn đang ở gần {stall.Name}. {stall.Specialty}. {stall.Description}";

            await TextToSpeech.Default.SpeakAsync(textToRead);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi TTS: {ex.Message}");
        }
        finally
        {
            _isSpeaking = false;
        }
    }

    public void ResetHistory()
    {
        _playedStallIds.Clear();
    }
}