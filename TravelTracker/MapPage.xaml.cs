using TravelTracker.Model;

namespace TravelTracker;

public partial class MapPage : ContentPage, IQueryAttributable
{
    private FoodStall _targetStall;

    public MapPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("TargetStall") && query["TargetStall"] is FoodStall stall)
        {
            _targetStall = stall;

            Title = $"Vị trí: {_targetStall.Name}";
            LoadMap(_targetStall.Latitude, _targetStall.Longitude, _targetStall.Name);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_targetStall == null)
        {
            LoadMap(10.7585, 106.7050, "Phố ẩm thực Vĩnh Khánh");
            Title = "Bản đồ ẩm thực";
        }
    }

    private void LoadMap(double lat, double lng, string name)
    {
        string searchQuery = System.Uri.EscapeDataString($"{name}");
        string mapUrl = $"https://www.google.com/maps/search/?api=1&query={searchQuery}&query_place_id={lat},{lng}&z=18&hl=vi";

        mapWebView.Source = mapUrl;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///MainPage");
    }
}