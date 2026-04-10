using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TravelTracker.Services;

public class LanguageService : INotifyPropertyChanged
{
    // Tạo ra một Instance duy nhất (Singleton) dùng chung cho toàn App
    private static readonly LanguageService _instance = new LanguageService();
    public static LanguageService Instance => _instance;

    private LanguageService() { }

    public string UIGreeting { get; private set; } = "Ăn gì hôm nay?";
    public string UIIntroHeading { get; private set; } = "GIỚI THIỆU TỔNG QUAN";
    public string UIPlayIntroBtn { get; private set; } = "🎧 Nghe giới thiệu tổng quan";
    public string UIStopIntroBtn { get; private set; } = "🛑 Dừng nghe giới thiệu";
    public string UISearchPlaceholder { get; private set; } = "🔍 Tìm quán ốc, lẩu, nướng...";
    public string UIListHeading { get; private set; } = "DANH SÁCH HÀNG QUÁN";
    public string UISpecialty { get; private set; } = "🍴 Đặc sản:";
    public string UIDetailBtn { get; private set; } = "Xem chi tiết & Nghe thuyết minh →";

    public void SetLanguage(string langCode)
    {
        if (langCode != null && langCode.StartsWith("en", StringComparison.OrdinalIgnoreCase))
        {
            UIGreeting = "What to eat today?";
            UIIntroHeading = "OVERVIEW";
            UIPlayIntroBtn = "🎧 Listen to overview";
            UIStopIntroBtn = "🛑 Stop listening";
            UISearchPlaceholder = "🔍 Search for seafood, BBQ...";
            UIListHeading = "FOOD STALLS";
            UISpecialty = "🍴 Specialty:";
            UIDetailBtn = "View details & Listen →";
        }
        else
        {
            UIGreeting = "Ăn gì hôm nay?";
            UIIntroHeading = "GIỚI THIỆU TỔNG QUAN";
            UIPlayIntroBtn = "🎧 Nghe giới thiệu tổng quan";
            UIStopIntroBtn = "🛑 Dừng nghe giới thiệu";
            UISearchPlaceholder = "🔍 Tìm quán ốc, lẩu, nướng...";
            UIListHeading = "DANH SÁCH HÀNG QUÁN";
            UISpecialty = "🍴 Đặc sản:";
            UIDetailBtn = "Xem chi tiết & Nghe thuyết minh →";
        }

        OnPropertyChanged(nameof(UIGreeting));
        OnPropertyChanged(nameof(UIIntroHeading));
        OnPropertyChanged(nameof(UIPlayIntroBtn));
        OnPropertyChanged(nameof(UIStopIntroBtn));
        OnPropertyChanged(nameof(UISearchPlaceholder));
        OnPropertyChanged(nameof(UIListHeading));
        OnPropertyChanged(nameof(UISpecialty));
        OnPropertyChanged(nameof(UIDetailBtn));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}