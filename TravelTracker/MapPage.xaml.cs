using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using Microsoft.Maui.Graphics;
using NetTopologySuite.Geometries;
using System.Net.Http;
using System.Text.Json;
using TravelTracker.Model;
using TravelTracker.Services;

namespace TravelTracker;

public partial class MapPage : ContentPage, IQueryAttributable
{
    private FoodStall _targetStall;
    private Tour _selectedTour;
    private readonly ApiService _apiService;
    private readonly GeofenceService _geofenceService;
    private IDispatcherTimer _locationTimer;
    private List<FoodStall> _cachedStalls;

    public MapPage()
    {
        InitializeComponent();

        _apiService = new ApiService();
        _geofenceService = new GeofenceService();

        _locationTimer = Application.Current.Dispatcher.CreateTimer();
        _locationTimer.Interval = TimeSpan.FromSeconds(5);
        _locationTimer.Tick += OnLocationTimerTicked;

        var tileSource = new BruTile.Web.HttpTileSource(
            new BruTile.Predefined.GlobalSphericalMercator(),
            "https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}.png",
            new[] { "a", "b", "c", "d" },
            "CartoVoyager");
        mapView.Map.Layers.Add(new Mapsui.Tiling.Layers.TileLayer(tileSource));

        mapView.PinClicked += async (s, e) =>
        {
            e.Handled = true;

            if (e.Pin?.Tag is FoodStall clickedStall)
            {
                string action = await DisplayActionSheet(
                    $"{clickedStall.Name}\n{clickedStall.Specialty}",
                    "Đóng bản đồ",
                    null,
                    "🌟 Xem chi tiết quán 🌟"
                );

                if (action == "🌟 Xem chi tiết quán 🌟")
                {
                    var navParam = new Dictionary<string, object>
                    {
                        { "SelectedStall", clickedStall }
                    };

                    await Shell.Current.GoToAsync("DetailPage", navParam);
                }
            }
        };
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("TargetStall") && query["TargetStall"] is FoodStall stall)
        {
            _targetStall = stall;
            _selectedTour = null;
            Title = _targetStall.Name;
        }
        else if (query.ContainsKey("SelectedTour") && query["SelectedTour"] is Tour tour)
        {
            _selectedTour = tour;
            _targetStall = null;
            Title = _selectedTour.Name;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        DeviceDisplay.Current.KeepScreenOn = true;

        var existingLayer = mapView.Map.Layers.FirstOrDefault(l => l.Name == "RouteLayer" || l.Name == "TourRouteLayer");
        if (existingLayer != null)
        {
            mapView.Map.Layers.Remove(existingLayer);
        }

        string currentLang = Preferences.Get("CurrentLanguage", "vi");
        _cachedStalls = await _apiService.GetFoodStallsAsync(currentLang);

        Microsoft.Maui.Devices.Sensors.Location myLocation = null;
        try
        {
            myLocation = await LocationService.GetCurrentLocationAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Lỗi lấy GPS: {ex.Message}");
        }

        LoadMap(_cachedStalls, myLocation);

        if (_selectedTour != null)
        {
            await DrawTourRouteAsync(_selectedTour);
        }
        else if (_targetStall != null && myLocation != null)
        {
            await DrawRouteAsync(myLocation.Latitude, myLocation.Longitude, _targetStall.Latitude, _targetStall.Longitude);
        }

        _locationTimer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _targetStall = null;
        _selectedTour = null;
        _locationTimer.Stop();

        DeviceDisplay.Current.KeepScreenOn = false;
        _geofenceService.CancelCurrentAudio();
    }

    private async void OnLocationTimerTicked(object sender, EventArgs e)
    {
        try
        {
            var myLocation = await LocationService.GetCurrentLocationAsync();

            if (myLocation != null && _cachedStalls != null)
            {
                UpdateUserPinOnMap(myLocation);

                string currentLang = Preferences.Get("CurrentLanguage", "vi");

                await _geofenceService.CheckAndTriggerAudioAsync(myLocation, _cachedStalls, currentLang, _apiService);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Lỗi radar: {ex.Message}");
        }
    }

    private void UpdateUserPinOnMap(Microsoft.Maui.Devices.Sensors.Location newLoc)
    {
        var existingUserPin = mapView.Pins.FirstOrDefault(p => p.Label == "Bạn đang ở đây");

        if (existingUserPin != null)
        {
            existingUserPin.Position = new Mapsui.UI.Maui.Position(newLoc.Latitude, newLoc.Longitude);
        }
        else
        {
            mapView.Pins.Add(new Pin(mapView)
            {
                Position = new Mapsui.UI.Maui.Position(newLoc.Latitude, newLoc.Longitude),
                Label = "Bạn đang ở đây",
                Type = PinType.Pin,
                Color = Colors.Blue,
                Scale = 0.8f
            });
        }
        mapView.RefreshGraphics();
    }

    private void LoadMap(List<FoodStall> allStalls, Microsoft.Maui.Devices.Sensors.Location myLocation)
    {
        if (allStalls == null) return;

        mapView.Pins.Clear();

        foreach (var stall in allStalls)
        {
            bool isTarget = (_targetStall != null && stall.Name == _targetStall.Name) ||
                            (_selectedTour != null && _selectedTour.TourItems.Any(ti => ti.FoodStallId == stall.Id));

            var pin = new Pin(mapView)
            {
                Position = new Mapsui.UI.Maui.Position(stall.Latitude, stall.Longitude),
                Label = stall.Name,
                Address = stall.Specialty,
                Type = PinType.Pin,
                Color = isTarget ? Colors.Orange : Colors.Red,
                Scale = 0.7f,
                Tag = stall
            };

            mapView.Pins.Add(pin);
        }

        double centerLat = _targetStall != null ? _targetStall.Latitude : 10.7607;
        double centerLng = _targetStall != null ? _targetStall.Longitude : 106.7033;

        var destPosition = SphericalMercator.FromLonLat(centerLng, centerLat);
        var centerPoint = new MPoint(destPosition.x, destPosition.y);

        double zoomLevel = _targetStall != null ? 1 : 2;
        mapView.Map.Navigator.CenterOnAndZoomTo(centerPoint, zoomLevel);

        if (myLocation != null)
        {
            UpdateUserPinOnMap(myLocation);
        }
    }


    private async Task DrawRouteAsync(double startLat, double startLng, double destLat, double destLng)
    {
        try
        {
            string url = $"https://routing.openstreetmap.de/routed-car/route/v1/driving/{startLng},{startLat};{destLng},{destLat}?overview=full&geometries=geojson";

            using var client = new HttpClient();
            var response = await client.GetStringAsync(url);
            using var document = JsonDocument.Parse(response);

            var routes = document.RootElement.GetProperty("routes");
            if (routes.GetArrayLength() == 0) return;

            var coordinates = routes[0].GetProperty("geometry").GetProperty("coordinates");

            var linePoints = new List<Coordinate>();
            foreach (var coord in coordinates.EnumerateArray())
            {
                var spherical = SphericalMercator.FromLonLat(coord[0].GetDouble(), coord[1].GetDouble());
                linePoints.Add(new Coordinate(spherical.x, spherical.y));
            }

            var feature = new GeometryFeature(new LineString(linePoints.ToArray()));
            feature.Styles.Add(new VectorStyle
            {
                Line = new Mapsui.Styles.Pen { Color = Mapsui.Styles.Color.FromString("#3498db"), Width = 6 }
            });

            mapView.Map.Layers.Add(new MemoryLayer { Name = "RouteLayer", Features = new[] { feature } });
            mapView.RefreshGraphics();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LỖI VẼ ĐƯỜNG: {ex.Message}");
        }
    }

    private async Task DrawTourRouteAsync(Tour tour)
    {
        try
        {
            if (tour?.TourItems == null || !tour.TourItems.Any()) return;

            var points = new List<Microsoft.Maui.Devices.Sensors.Location>();
            foreach (var item in tour.TourItems.OrderBy(ti => ti.OrderIndex))
            {
                if (item.FoodStall != null)
                {
                    points.Add(new Microsoft.Maui.Devices.Sensors.Location(item.FoodStall.Latitude, item.FoodStall.Longitude));
                }
            }

            if (points.Count < 2) return;

            var routeLocations = await _apiService.GetRouteAsync(points);
            if (routeLocations == null || !routeLocations.Any()) return;

            var linePoints = new List<Coordinate>();
            foreach (var loc in routeLocations)
            {
                var spherical = SphericalMercator.FromLonLat(loc.Longitude, loc.Latitude);
                linePoints.Add(new Coordinate(spherical.x, spherical.y));
            }

            var feature = new GeometryFeature(new LineString(linePoints.ToArray()));
            feature.Styles.Add(new VectorStyle
            {
                Line = new Mapsui.Styles.Pen { Color = Mapsui.Styles.Color.FromString("#FF5722"), Width = 6 }
            });

            mapView.Map.Layers.Add(new MemoryLayer { Name = "TourRouteLayer", Features = new[] { feature } });

            var firstStop = points.First();
            var centerPosition = SphericalMercator.FromLonLat(firstStop.Longitude, firstStop.Latitude);
            mapView.Map.Navigator.CenterOnAndZoomTo(new MPoint(centerPosition.x, centerPosition.y), 1.5);

            mapView.RefreshGraphics();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LỖI VẼ TOUR: {ex.Message}");
        }
    }
}