using System.Text.RegularExpressions;
using TravelTracker.Model;

namespace TravelTracker;

public partial class DetailPage : ContentPage, IQueryAttributable
{
    private FoodStall _currentStall;
    private LanguageOption _currentLanguage;
    CancellationTokenSource cts;
    string[] sentences;
    int currentIndex = 0;
    bool isReading = false;

    public DetailPage()
    {
        InitializeComponent();
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("SelectedStall") && query["SelectedStall"] is FoodStall stall)
        {
            _currentStall = stall;
            BindingContext = _currentStall;

            if (!string.IsNullOrEmpty(_currentStall.Description))
            {
                sentences = Regex.Split(_currentStall.Description, @"(?<=[.!?])\s+");
            }
        }

        if (query.ContainsKey("SelectedLanguage") && query["SelectedLanguage"] is LanguageOption lang)
        {
            _currentLanguage = lang;
        }

        currentIndex = 0;
        isReading = false;
    }

    private async void OnListenClicked(object sender, EventArgs e)
    {
        if (isReading || sentences == null || sentences.Length == 0) return;

        if (cts != null) cts.Cancel();
        cts = new CancellationTokenSource();
        isReading = true;

        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var targetLocale = locales.FirstOrDefault(l => l.Language.StartsWith(_currentLanguage.LanguageCode));
            var speechOptions = new SpeechOptions() { Locale = targetLocale };

            for (int i = currentIndex; i < sentences.Length; i++)
            {
                currentIndex = i;

                // Đọc với tùy chọn ngôn ngữ đã cài đặt
                await TextToSpeech.Default.SpeakAsync(sentences[i], speechOptions, cancelToken: cts.Token);

                if (cts.Token.IsCancellationRequested)
                    break;
            }

            if (!cts.Token.IsCancellationRequested && currentIndex >= sentences.Length - 1)
            {
                currentIndex = 0;
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            isReading = false;
        }
    }

    //Hàm xử lý button Stop
    private void OnStopClicked(object sender, EventArgs e)
    {
        if (cts != null)
        {
            cts.Cancel();
            isReading = false;
        }
    }
    private void OnResetClicked(object sender, EventArgs e)
    {
        OnStopClicked(sender, e);
        currentIndex = 0;
        DisplayAlert("Thông báo", "Đã đặt lại vị trí đọc về ban đầu.", "OK");
    }

    // Tự động tắt tiếng khi người dùng thoát trang
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (cts != null) cts.Cancel();
    }
}
