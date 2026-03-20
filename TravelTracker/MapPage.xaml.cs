namespace TravelTracker;

public partial class MapPage : ContentPage
{
	public MapPage()
	{
		InitializeComponent();
	}
    private async void OnViewDetailClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//DetailPage");
    }
}