using System.Collections.ObjectModel;
using System.Linq;
using TravelTracker.Model;
using TravelTracker.Services;
using System.Text.RegularExpressions;

namespace TravelTracker;

public partial class MainPage : ContentPage
{
    private readonly ApiService _apiService;

    public ObservableCollection<FoodStall> Stalls { get; set; }
    public ObservableCollection<Language> Languages { get; set; }

    private Language _selectedLanguage;
    public Language SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (_selectedLanguage != value)
            {
                _selectedLanguage = value;
                OnPropertyChanged();

                if (isReadingIntro) StopIntro();
                introSentences = null;
                currentIntroIndex = 0;

                if (_selectedLanguage != null)
                {
                    _ = LoadStallsFromApiAsync(_selectedLanguage.LanguageCode);
                }
            }
        }
    }

    CancellationTokenSource ctsIntro;
    string[] introSentences;
    int currentIntroIndex = 0;
    bool isReadingIntro = false;

    public MainPage()
    {
        InitializeComponent();

        _apiService = new ApiService();

        Languages = new ObservableCollection<Language>();
        Stalls = new ObservableCollection<FoodStall>();

        _ = LoadLanguagesFromApiAsync();

        BindingContext = this;
    }

    private async Task LoadStallsFromApiAsync(string langCode)
    {
        var stallsFromBE = await _apiService.GetFoodStallsAsync(langCode);

        Stalls.Clear();
        foreach (var stall in stallsFromBE)
        {
            Stalls.Add(stall);
        }

        _ = UpdateStallDistancesAsync();
    }

    private async Task LoadLanguagesFromApiAsync()
    {
        var langList = await _apiService.GetLanguagesAsync();

        if (langList != null && langList.Count > 0)
        {
            foreach (var lang in langList)
            {
                Languages.Add(lang);
            }

            SelectedLanguage = Languages[0];
        }
    }
    private async Task UpdateStallDistancesAsync()
    {
        var myLocation = await LocationService.GetCurrentLocationAsync();

        if (myLocation == null || Stalls == null || Stalls.Count == 0)
            return;

        foreach (var stall in Stalls)
        {
            var stallLocation = new Location(stall.Latitude, stall.Longitude);
            double distance = Location.CalculateDistance(myLocation, stallLocation, DistanceUnits.Kilometers);
            stall.DistanceText = $"📍 Cách {Math.Round(distance, 1)} km";
        }
    }

    private async void OnGoToMapClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        if (button?.CommandParameter is FoodStall stall)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "TargetStall", stall }
            };

            await Shell.Current.GoToAsync("//MapPage", navigationParameter);
        }
    }

    private async void OnStallSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedStall = e.CurrentSelection.FirstOrDefault() as FoodStall;
        if (selectedStall != null)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "SelectedStall", selectedStall },
                { "SelectedLanguage", SelectedLanguage }
            };
            await Shell.Current.GoToAsync("DetailPage", navigationParameter);
        }
        ((CollectionView)sender).SelectedItem = null;
    }

    private async void OnStartTrackingClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MapPage");
    }

    private async void OnToggleStreetIntroClicked(object sender, EventArgs e)
    {
        var button = sender as Button;

        if (isReadingIntro)
        {
            StopIntro(button);
            return;
        }

        if (introSentences == null && SelectedLanguage != null)
            introSentences = Regex.Split(SelectedLanguage.IntroText, @"(?<=[.!?])\s+");

        if (ctsIntro != null)
        {
            ctsIntro.Cancel();
            ctsIntro.Dispose();
            ctsIntro = null;
        }

        ctsIntro = new CancellationTokenSource();
        isReadingIntro = true;

        button.Text = "🛑 Dừng nghe giới thiệu";
        button.BackgroundColor = Colors.Red;

        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var targetLocale = locales.FirstOrDefault(l => l.Language.StartsWith(SelectedLanguage.LanguageCode));
            var speechOptions = new SpeechOptions() { Locale = targetLocale };

            for (int i = currentIntroIndex; i < introSentences.Length; i++)
            {
                currentIntroIndex = i;
                double startProgress = (double)i / introSentences.Length;
                double targetProgress = (double)(i + 1) / introSentences.Length;
                uint estimatedDuration = (uint)(introSentences[i].Length * 65);

                var counterAnimation = new Animation(v => {
                    lblProgressPercent.Text = $"{(int)v}%";
                }, startProgress * 100, targetProgress * 100);

                introProgressBar.ProgressTo(targetProgress, estimatedDuration, Easing.Linear);
                counterAnimation.Commit(this, "PercentAnimation", 16, estimatedDuration, Easing.Linear);

                await TextToSpeech.Default.SpeakAsync(introSentences[i], speechOptions, cancelToken: ctsIntro.Token);

                if (ctsIntro.Token.IsCancellationRequested) break;
            }

            if (currentIntroIndex >= introSentences.Length - 1 && !ctsIntro.Token.IsCancellationRequested)
            {
                currentIntroIndex = 0;
                introProgressBar.Progress = 0;
                lblProgressPercent.Text = "0%";
                ResetButtonUI(button);
            }
        }
        catch (OperationCanceledException) { }
        finally { isReadingIntro = false; }
    }

    private void StopIntro(Button button = null)
    {
        if (ctsIntro != null)
        {
            ctsIntro.Cancel();
            ctsIntro.Dispose();
            ctsIntro = null;

            isReadingIntro = false;

            if (button != null)
            {
                button.Text = "▶️ Nghe tiếp giới thiệu";
                button.BackgroundColor = Color.FromArgb("#512BD4");
            }
            else if (btnToggleIntro != null)
                ResetButtonUI(btnToggleIntro);
        }
    }

    private void ResetButtonUI(Button button)
    {
        if (button == null) return;
        button.Text = "🎧 Nghe giới thiệu tổng quan";
        button.BackgroundColor = Color.FromArgb("#512BD4");
        if (currentIntroIndex == 0)
        {
            introProgressBar.Progress = 0;
            lblProgressPercent.Text = "0%";
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (isReadingIntro) StopIntro();

        if (ctsIntro != null)
        {
            ctsIntro.Cancel();
            ctsIntro.Dispose();
            ctsIntro = null;
        }
    }
}