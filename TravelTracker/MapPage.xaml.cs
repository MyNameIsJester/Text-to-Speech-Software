using System.Text.Json;
using System.Net.Http;
using TravelTracker.Model;
using Mapsui;
using Mapsui.Projections;
using Mapsui.UI.Maui;
using Mapsui.Tiling;
using Microsoft.Maui.Graphics;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Nts;
using NetTopologySuite.Geometries;

namespace TravelTracker;

public partial class MapPage : ContentPage, IQueryAttributable
{
    private FoodStall _targetStall;

    public MapPage()
    {
        InitializeComponent();

        mapView.Map?.Layers.Add(OpenStreetMap.CreateTileLayer());

        mapView.PinClicked += (s, e) =>
        {
            e.Handled = true;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("📍 Thông tin", e.Pin.Label, "Đóng");
            });
        };
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("TargetStall") && query["TargetStall"] is FoodStall stall)
        {
            _targetStall = stall;
            Title = _targetStall.Name;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var myLocation = await LocationService.GetCurrentLocationAsync();

        double destLat = _targetStall != null ? _targetStall.Latitude : 10.7607;
        double destLng = _targetStall != null ? _targetStall.Longitude : 106.7033;
        string destName = _targetStall != null ? _targetStall.Name : "Phố ẩm thực Vĩnh Khánh";

        LoadMap(destLat, destLng, destName, myLocation);

        if (myLocation != null)
        {
            await DrawRouteAsync(myLocation.Latitude, myLocation.Longitude, destLat, destLng);
        }
    }

    private void LoadMap(double destLat, double destLng, string destName, Microsoft.Maui.Devices.Sensors.Location myLocation)
    {
        var destPosition = SphericalMercator.FromLonLat(destLng, destLat);
        var centerPoint = new MPoint(destPosition.x, destPosition.y);

        mapView.Map.Navigator.CenterOnAndZoomTo(centerPoint, 1);
        mapView.Pins.Clear();

        mapView.Pins.Add(new Pin(mapView)
        {
            Position = new Mapsui.UI.Maui.Position(destLat, destLng),
            Label = destName,
            Type = PinType.Pin,
            Color = Microsoft.Maui.Graphics.Colors.Red,
            Scale = 0.8f
        });

        if (myLocation != null)
        {
            mapView.Pins.Add(new Pin(mapView)
            {
                Position = new Mapsui.UI.Maui.Position(myLocation.Latitude, myLocation.Longitude),
                Label = "Bạn đang ở đây",
                Type = PinType.Pin,
                Color = Microsoft.Maui.Graphics.Colors.Blue,
                Scale = 0.8f
            });
        }
    }

    private async Task DrawRouteAsync(double startLat, double startLng, double destLat, double destLng)
    {
        try
        {
            var existingLayer = mapView.Map.Layers.FirstOrDefault(l => l.Name == "RouteLayer");
            if (existingLayer != null) mapView.Map.Layers.Remove(existingLayer);

            string url = $"https://routing.openstreetmap.de/routed-car/route/v1/driving/{startLng},{startLat};{destLng},{destLat}?overview=full&geometries=geojson";

            using var client = new HttpClient();
            var response = await client.GetStringAsync(url);
            using var document = JsonDocument.Parse(response);

            var coordinates = document.RootElement.GetProperty("routes")[0]
                                      .GetProperty("geometry")
                                      .GetProperty("coordinates");

            var linePoints = new List<Coordinate>();
            foreach (var coord in coordinates.EnumerateArray())
            {
                var spherical = SphericalMercator.FromLonLat(coord[0].GetDouble(), coord[1].GetDouble());
                linePoints.Add(new Coordinate(spherical.x, spherical.y));
            }

            var feature = new GeometryFeature(new LineString(linePoints.ToArray()));
            feature.Styles.Add(new VectorStyle
            {
                Line = new Pen { Color = Mapsui.Styles.Color.FromString("#3498db"), Width = 6 }
            });

            mapView.Map.Layers.Add(new MemoryLayer { Name = "RouteLayer", Features = new[] { feature } });
            mapView.RefreshGraphics();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LỖI VẼ ĐƯỜNG OSRM: {ex.Message}");
        }
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///MainPage");
    }
}