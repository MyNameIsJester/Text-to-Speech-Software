using System.Collections.ObjectModel;
using System.Linq;
using TravelTracker.Model;
using System.Text.RegularExpressions;

namespace TravelTracker;

public partial class MainPage : ContentPage
{
    //Tạo ds các quán ăn
    public ObservableCollection<FoodStall> Stalls { get; set; }

    // Quản lý trạng thái Audio cho phần giới thiệu chung
    CancellationTokenSource ctsIntro;
    string[] introSentences;   
    int currentIntroIndex = 0;
    bool isReadingIntro = false;

    string introFullText = "The Vĩnh Khánh Food Street is one of the most vibrant street food destinations in Ho Chi Minh City, especially famous for its lively nightlife and authentic local flavors. In the evening, the street comes alive with bright lights, bustling crowds, and the irresistible aroma of grilled seafood. Visitors can experience a truly local atmosphere, sitting on small plastic stools along the sidewalk, enjoying food in an open, energetic setting. Vĩnh Khánh is best known for its wide variety of dishes, particularly fresh seafood such as snails, clams, grilled shrimp, and squid, all prepared with bold Vietnamese spices. It’s also a great place to try popular street snacks and enjoy a casual “eat and share” dining style, often accompanied by cold drinks. More than just a place to eat, the street offers a glimpse into local culture—friendly, social, and full of life. Affordable prices and diverse options make it ideal for travelers who want to explore multiple dishes in one visit. Located just a few minutes from the city center, it is easily accessible and a must visit for anyone seeking an authentic taste of Saigon’s street food scene.";

    public MainPage()
    {
        InitializeComponent();

        //Các dữ liệu ảo về các quán ăn tiêu biểu trên phố Vĩnh Khánh
        Stalls = new ObservableCollection<FoodStall>
        {
            new FoodStall { Name = "Ốc Oanh", Address = "534 Vĩnh Khánh", Specialty = "Ốc hương rang muối, Càng ghẹ", PriceRange = "50.000đ - 200.000đ", ImageUrl = "oc_oanh.jpg", Description = "Ốc Oanh là một trong những quán ốc nổi tiếng nhất tại phố Vĩnh Khánh, nổi bật với không gian nhộn nhịp và các món hải sản tươi ngon được chế biến đậm đà." },
            new FoodStall { Name = "Lẩu Bò Khu Nhà Cháy", Address = "001 Chung cư Vĩnh Khánh", Specialty = "Lẩu bò thập cẩm, Bò nướng mắm nhĩ", PriceRange = "150.000đ - 300.000đ", ImageUrl = "lau_bo_nha_chay.png", Description = "Hương vị lẩu bò truyền thống với nước dùng thanh ngọt, thịt bò mềm và các loại rau ăn kèm phong phú." },
            new FoodStall { Name = "Ốc Vũ", Address = "139 Vĩnh Khánh", Specialty = "Ốc len xào dừa, Sò huyết xào tỏi", PriceRange = "40.000đ - 150.000đ", ImageUrl = "oc_vu.jpg", Description = "Quán có không gian rộng rãi, phục vụ nhanh chóng với thực đơn đa dạng các loại ốc và hải sản bình dân." }
        };

        BindingContext = this;
    }
    // Hàm xử lý khi người dùng nhấn vào một quán ăn trong danh sách
    private async void OnStallSelected(object sender, SelectionChangedEventArgs e)
    {
        // Kiểm tra xem có quán nào được chọn không
        var selectedStall = e.CurrentSelection.FirstOrDefault() as FoodStall;

        if (selectedStall != null)
        {
            // Tạo trang chứa dữ liệu quán ăn 
            var navigationParameter = new Dictionary<string, object>
            {
                { "SelectedStall", selectedStall }
            };

            // Lệnh chuyển trang
            await Shell.Current.GoToAsync("//DetailPage", navigationParameter);
        }

        // Reset lại lựa chọn (vô cùng quan trọng để người dùng có thể bấm lại chính quán đó)
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
            StopIntro();
            return;
        }

        if (introSentences == null)
        {
            introSentences = Regex.Split(introFullText, @"(?<=[.!?])\s+");
        }

        if (ctsIntro != null) ctsIntro.Cancel();
        ctsIntro = new CancellationTokenSource();

        isReadingIntro = true;

        // Cập nhật giao diện nút bấm
        btnToggleIntro.Text = "🛑 Dừng nghe giới thiệu";
        btnToggleIntro.BackgroundColor = Colors.Red;

        try
        {
            for (int i = currentIntroIndex; i < introSentences.Length; i++)
            {
                currentIntroIndex = i; // Lưu vị trí câu đang đọc
                await TextToSpeech.Default.SpeakAsync(introSentences[i], cancelToken: ctsIntro.Token);

                if (ctsIntro.Token.IsCancellationRequested) break;
            }

            // Nếu đọc xong hết cả bài
            if (currentIntroIndex >= introSentences.Length - 1 && !ctsIntro.Token.IsCancellationRequested)
            {
                currentIntroIndex = 0;
                ResetButtonUI(button);
            }
        }
        catch (OperationCanceledException) 
        { 
        
        }

        finally 
        { 
            isReadingIntro = false; 
        }
    }

    private void StopIntro(Button button = null)
    {
        if (ctsIntro != null)
        {
            ctsIntro.Cancel();
            isReadingIntro = false;

            // Nếu dừng giữa chừng, đổi chữ thành "Nghe tiếp"
            if (button != null)
            {
                button.Text = "▶️ Nghe tiếp giới thiệu";
                button.BackgroundColor = Color.FromArgb("#512BD4");
            }
            else if (btnToggleIntro != null) // Trường hợp gọi từ OnDisappearing
            {
                ResetButtonUI(btnToggleIntro);
            }
        }
    }

    private void ResetButtonUI(Button button)
    {
        if (button == null) return;
        button.Text = "🎧 Nghe giới thiệu tổng quan";
        button.BackgroundColor = Color.FromArgb("#512BD4");
    }

    // Tự động dừng khi chuyển trang
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (isReadingIntro)
        {
            StopIntro(); // Dừng nhưng giữ nguyên currentIntroIndex để lần sau quay lại nghe tiếp
        }
    }
}