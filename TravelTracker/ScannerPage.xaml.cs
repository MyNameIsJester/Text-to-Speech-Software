using ZXing.Net.Maui;
using TravelTracker.Services;
using TravelTracker.Model;

namespace TravelTracker;

public partial class ScannerPage : ContentPage
{
    private readonly ApiService _apiService;
    private bool _isProcessing = false;

    public ScannerPage()
    {
        InitializeComponent();
        _apiService = new ApiService();

        cameraReader.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = false
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _isProcessing = false;

        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Lỗi", "Bạn cần cấp quyền Camera để quét QR.", "OK");
                return;
            }
        }

        cameraReader.IsDetecting = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        cameraReader.IsDetecting = false;
    }

    private void CameraReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (_isProcessing) return;
        _isProcessing = true;

        cameraReader.IsDetecting = false;

        var result = e.Results?.FirstOrDefault();
        if (result != null)
        {
            string qrContent = result.Value;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var stalls = await _apiService.GetFoodStallsAsync("vi");

                var matchedStall = stalls.FirstOrDefault(s => s.Name.Equals(qrContent, StringComparison.OrdinalIgnoreCase));

                if (matchedStall != null)
                {
                    await DisplayAlert("Thành công", $"Đã tìm thấy: {matchedStall.Name}. Chuẩn bị phát thuyết minh...", "Nghe ngay");

                    var navParam = new Dictionary<string, object> { { "SelectedStall", matchedStall } };
                    await Shell.Current.GoToAsync("DetailPage", navParam);
                }
                else
                {
                    await DisplayAlert("Không hợp lệ", $"Mã QR này ({qrContent}) không có trong hệ thống trạm xe buýt.", "Thử lại");
                    _isProcessing = false;
                    cameraReader.IsDetecting = true;
                }
            });
        }
    }
}