using System.Text.RegularExpressions;
using TravelTracker.Model;

namespace TravelTracker;

[QueryProperty(nameof(CurrentStall), "SelectedStall")]
public partial class DetailPage : ContentPage
{
    private FoodStall _currentStall;
    public FoodStall CurrentStall
    {
        get => _currentStall;
        set
        {
            _currentStall = value;
            // Khi nhận được dữ liệu, gán nó vào giao diện (Binding)
            BindingContext = _currentStall;

            if (_currentStall != null && !string.IsNullOrEmpty(_currentStall.Description))
            {
                sentences = Regex.Split(_currentStall.Description, @"(?<=[.!?])\s+");
            }

            // Reset lại vị trí đọc và trạng thái khi chuyển sang quán mới
            currentIndex = 0;
            isReading = false;
        }
    }

    //Khai báo bộ điều khiển hủy bỏ
    CancellationTokenSource cts;
    string[] sentences;       // Danh sách các câu đã chia nhỏ
    int currentIndex = 0;    // Lưu vị trí câu đang đọc
    bool isReading = false; // Trạng thái để kiểm tra đang đọc hay dừng

    public DetailPage()
	{
		InitializeComponent();
    }

    private async void OnListenClicked(object sender, EventArgs e)
    {

        if (isReading || sentences == null || sentences.Length == 0) return;

        // Hủy lệnh cũ nếu có
        if (cts != null) cts.Cancel();
        cts = new System.Threading.CancellationTokenSource();

        isReading = true;

        try
        {
            for (int i = currentIndex; i < sentences.Length; i++)
            {
                currentIndex = i; //cập nhật vị trí hiện tại

                //Theo dõi App đọc
                System.Diagnostics.Debug.WriteLine($"Đang đọc câu số: {i}");

                //đọc câu tương ứng 
                await TextToSpeech.Default.SpeakAsync(sentences[i], cancelToken: cts.Token);

                // Kiểm tra
                if (cts.Token.IsCancellationRequested)
                    break;
            }

            // Nếu đã đọc xong câu cuối cùng, tự động quay về câu đầu
            if (!cts.Token.IsCancellationRequested && currentIndex >= sentences.Length - 1)
            {
                currentIndex = 0;
            }
        }
        catch (OperationCanceledException)
        {

        }
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
            cts.Cancel(); // Ra lệnh dừng ngay lập tức
            isReading = false;
        }
    }
    private void OnResetClicked(object sender, EventArgs e)
    {
        OnStopClicked(sender, e);
        currentIndex = 0;
        DisplayAlert("Thông báo", "Đã đặt lại vị trí đọc về ban đầu.", "OK");
    }
}
