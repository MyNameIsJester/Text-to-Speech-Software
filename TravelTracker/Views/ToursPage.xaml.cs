using TravelTracker.Model;
using TravelTracker.Services;

namespace TravelTracker.Views;

public partial class ToursPage : ContentPage
{
    private readonly ApiService _apiService;

    public ToursPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadToursAsync();
    }

    private async Task LoadToursAsync()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        ToursCollectionView.IsVisible = false;

        string currentLang = Preferences.Get("CurrentLanguage", "vi");
        var tours = await _apiService.GetToursAsync(currentLang);

        ToursCollectionView.ItemsSource = tours;

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
        ToursCollectionView.IsVisible = true;
    }

    private async void OnTourSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Tour selectedTour)
        {
            ((CollectionView)sender).SelectedItem = null;

            var navigationParams = new Dictionary<string, object>
            {
                { "SelectedTour", selectedTour }
            };

            await Shell.Current.GoToAsync("///MapPage", navigationParams);
        }
    }
}