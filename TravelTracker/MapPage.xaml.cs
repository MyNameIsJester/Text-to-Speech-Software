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
            // Chỉ lưu dữ liệu và đổi Title, KHÔNG gọi LoadMap ở đây nữa
            Title = $"Vị trí: {_targetStall.Name}";
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_targetStall != null)
        {
            // Để OnAppearing đảm nhiệm việc tải bản đồ khi giao diện đã sẵn sàng
            LoadMap(_targetStall.Latitude, _targetStall.Longitude, _targetStall.Name);
        }
        else
        {
            LoadMap(10.7607, 106.7033, "Phố ẩm thực Vĩnh Khánh");
            Title = "Bản đồ ẩm thực";
        }
    }

    /* public void ApplyQueryAttributes(IDictionary<string, object> query)
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
    */
    private void LoadMap(double lat, double lng, string name)
    {
        // 1. Ép kiểu tọa độ dùng dấu chấm (chuẩn quốc tế)
        var culture = System.Globalization.CultureInfo.InvariantCulture;
        string latStr = lat.ToString(culture);
        string lngStr = lng.ToString(culture);

        // 2. Mã hóa tên quán (tránh lỗi font tiếng Việt khi đưa lên web)
        string encodedName = System.Uri.EscapeDataString(name);

        // 3. ĐƯỜNG LINK CHUẨN CỦA GOOGLE MAPS (Hiện đúng Ghim + Tên quán)
        // Cú pháp: q={lat},{lng}+({Tên})
        string mapUrl = $"https://maps.google.com/maps?q={latStr},{lngStr}+({encodedName})&hl=vi&z=18&output=embed";

        // 4. Bọc vào HTML để hiển thị toàn màn hình
        string htmlContent = $@"
    <html>
    <head>
        <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no' />
        <style>
            body, html {{ margin: 0; padding: 0; height: 100%; width: 100%; overflow: hidden; }}
            iframe {{ border: none; width: 100%; height: 100%; }}
        </style>
    </head>
    <body>
        <iframe src='{mapUrl}' allowfullscreen></iframe>
    </body>
    </html>";

        mapWebView.Source = new HtmlWebViewSource { Html = htmlContent };
    }

    /*  private void LoadMap(double lat, double lng, string name)
      {
          string searchQuery = System.Uri.EscapeDataString($"{name}");
          string mapUrl = $"https://www.google.com/maps/search/?api=1&query={searchQuery}&query_place_id={lat},{lng}&z=18&hl=vi";

          mapWebView.Source = mapUrl;
      }
    */

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///MainPage");
    }
}