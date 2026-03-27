using System.Collections.ObjectModel;
using System.Linq;
using TravelTracker.Model;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;

namespace TravelTracker;

public partial class MainPage : ContentPage
{
    // Tạo ds các quán ăn
    public ObservableCollection<FoodStall> Stalls { get; set; }
    public ObservableCollection<LanguageOption> Languages { get; set; }

    private LanguageOption _selectedLanguage;
    public LanguageOption SelectedLanguage
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

                if (_selectedLanguage?.Stalls != null)
                {
                    Stalls.Clear();
                    foreach (var stall in _selectedLanguage.Stalls)
                    {
                        Stalls.Add(stall);
                    }
                }
            }
        }
    }

    // Quản lý trạng thái Audio cho phần giới thiệu chung
    CancellationTokenSource ctsIntro;
    string[] introSentences;
    int currentIntroIndex = 0;
    bool isReadingIntro = false;

    //string introFullText = "The Vĩnh Khánh Food Street is one of the most vibrant street food destinations in Ho Chi Minh City, especially famous for its lively nightlife and authentic local flavors. In the evening, the street comes alive with bright lights, bustling crowds, and the irresistible aroma of grilled seafood. Visitors can experience a truly local atmosphere, sitting on small plastic stools along the sidewalk, enjoying food in an open, energetic setting. Vĩnh Khánh is best known for its wide variety of dishes, particularly fresh seafood such as snails, clams, grilled shrimp, and squid, all prepared with bold Vietnamese spices. It’s also a great place to try popular street snacks and enjoy a casual “eat and share” dining style, often accompanied by cold drinks. More than just a place to eat, the street offers a glimpse into local culture—friendly, social, and full of life. Affordable prices and diverse options make it ideal for travelers who want to explore multiple dishes in one visit. Located just a few minutes from the city center, it is easily accessible and a must visit for anyone seeking an authentic taste of Saigon’s street food scene.";

    public MainPage()
    {
        InitializeComponent();

        Languages = new ObservableCollection<LanguageOption>();
        Stalls = new ObservableCollection<FoodStall>();

        _ = LoadLanguageDataAsync();

        BindingContext = this;
    }
    private async Task LoadLanguageDataAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("languages.json");
            using var reader = new StreamReader(stream);
            var jsonContent = await reader.ReadToEndAsync();

            var languageList = JsonSerializer.Deserialize<List<LanguageOption>>(jsonContent);

            if (languageList != null)
            {
                foreach (var lang in languageList)
                {
                    Languages.Add(lang);
                }

                if (Languages.Count > 0)
                {
                    SelectedLanguage = Languages[0]; // Khi gán, nó sẽ tự động chạy lệnh Set ở trên để nạp Stalls
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi đọc JSON: {ex.Message}");
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

        if (ctsIntro != null) ctsIntro.Cancel();
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
    }
}