# PRD - Hệ thống Audio Guide Phố Ẩm Thực Vĩnh Khánh

## 1. Giới thiệu sản phẩm

### 1.1. Tên sản phẩm

**Audio Guide Phố Ẩm Thực Vĩnh Khánh**

### 1.2. Mô tả bài toán

Phố Ẩm Thực Vĩnh Khánh tập trung nhiều quán ăn đặc trưng, nhưng trải nghiệm khám phá của du khách hiện vẫn phụ thuộc nhiều vào việc tự tìm kiếm thông tin hoặc đọc nội dung thủ công. Điều này làm cho quá trình tham quan bị rời rạc, thiếu tính dẫn dắt và khó tạo được trải nghiệm số liền mạch tại điểm đến.

Hệ thống được xây dựng nhằm giải quyết bài toán cung cấp nội dung giới thiệu địa điểm theo ngữ cảnh vị trí thực tế của người dùng. Khi du khách di chuyển đến gần một quán ăn hoặc một điểm quan tâm, ứng dụng sẽ tự động phát nội dung thuyết minh bằng âm thanh mà không yêu cầu người dùng thao tác nhiều trên màn hình.

### 1.3. Mục tiêu sản phẩm

Mục tiêu của hệ thống là xây dựng một giải pháp hướng dẫn tham quan ẩm thực có khả năng hoạt động tự động dựa trên vị trí GPS, hỗ trợ nhiều ngôn ngữ, dễ quản trị nội dung và phù hợp với quy mô triển khai của một đồ án có tính ứng dụng thực tế.

Các mục tiêu chính bao gồm:

| Mã | Mục tiêu | Diễn giải |
|---|---|---|
| G1 | Tự động phát thuyết minh theo vị trí | Ứng dụng phát nội dung audio khi người dùng đi vào vùng kích hoạt của địa điểm |
| G2 | Hỗ trợ khám phá địa điểm ẩm thực | Người dùng có thể xem danh sách quán, vị trí và thông tin mô tả |
| G3 | Hỗ trợ đa ngôn ngữ | Nội dung quán và tour có thể hiển thị theo ngôn ngữ được chọn |
| G4 | Hỗ trợ tour nhiều điểm | Người dùng có thể tham gia tour gồm nhiều quán được sắp theo thứ tự |
| G5 | Hỗ trợ quản trị nội dung | Quản trị viên có thể quản lý quán, bản dịch, tour và theo dõi log |
| G6 | Duy trì kiến trúc dễ mở rộng | Hệ thống có thể mở rộng dữ liệu, nội dung và tính năng trong các giai đoạn tiếp theo |

### 1.4. Giá trị mang lại

| Nhóm giá trị | Nội dung |
|---|---|
| Trải nghiệm người dùng | Giảm thao tác thủ công, tăng tính tự nhiên trong quá trình khám phá |
| Giá trị nội dung | Cung cấp thông tin mô tả địa điểm dưới dạng âm thanh, phù hợp bối cảnh tham quan |
| Giá trị vận hành | Cho phép quản lý dữ liệu tập trung qua Web Admin |
| Giá trị kỹ thuật | Kiến trúc tách thành app, API, database và admin, thuận lợi cho bảo trì và mở rộng |

### 1.5. Định hướng phiên bản hiện tại

Phiên bản hiện tại của hệ thống tập trung vào hai trục chức năng chính:

1. **GPS-based trigger** để phát nội dung tự động theo vị trí.
2. **Text-to-Speech / audio playback flow** để cung cấp trải nghiệm nghe giới thiệu tại điểm đến.

Trong phạm vi hiện tại, hệ thống không còn lấy QR làm cơ chế kích hoạt nội dung chính. Toàn bộ trọng tâm trải nghiệm được chuyển sang mô hình GPS kết hợp phát audio tự động.

---

## 2. Phạm vi và mục tiêu triển khai

### 2.1. Phạm vi chức năng

Hệ thống bao gồm bốn thành phần chính:

| Thành phần | Vai trò chính |
|---|---|
| Mobile App (.NET MAUI) | Ứng dụng cho người dùng cuối, xử lý GPS, hiển thị dữ liệu và phát audio |
| Backend API (ASP.NET Core Web API) | Cung cấp dữ liệu cho mobile app, nhận và lưu playback log |
| Database (SQLite) | Lưu trữ dữ liệu địa điểm, bản dịch, tour, ngôn ngữ và lịch sử phát |
| Web Admin (ASP.NET Core MVC) | Quản trị nội dung và theo dõi dữ liệu vận hành |

### 2.2. Phạm vi trong phiên bản hiện tại

Các chức năng nằm trong phạm vi của phiên bản hiện tại gồm:

| Nhóm chức năng | Nội dung |
|---|---|
| Quản lý địa điểm | Lưu trữ và quản lý các quán ăn dưới dạng POI |
| Quản lý đa ngôn ngữ | Hỗ trợ translation cho quán ăn và tour |
| Truy xuất dữ liệu cho mobile | API trả danh sách quán, chi tiết quán, danh sách ngôn ngữ và tour |
| Ghi nhận lịch sử phát | App gửi playback log về backend để lưu trữ |
| Quản lý tour | Tạo, chỉnh sửa, kích hoạt và quản lý các tour nhiều điểm |
| Dashboard quản trị | Hiển thị số liệu tổng quan và hoạt động phát gần đây |
| Quản trị người dùng | Tạo tài khoản và gán role cho người dùng Web Admin |

### 2.3. Ngoài phạm vi

Các nội dung sau không phải trọng tâm của phiên bản hiện tại:

| Nội dung | Ghi chú |
|---|---|
| Trigger nội dung bằng QR | Không sử dụng trong phạm vi nghiệp vụ chính hiện tại |
| Streaming audio thời gian thực từ server | Hệ thống hiện thiên về phát nội dung từ dữ liệu audio/TTS phía ứng dụng |
| Đồng bộ thời gian thực giữa nhiều thiết bị | Không phải yêu cầu chính của phiên bản hiện tại |
| Hệ quản trị cơ sở dữ liệu quy mô lớn | SQLite phù hợp hơn với quy mô hiện tại của đồ án |
| Cơ chế recommendation hoặc cá nhân hóa nâng cao | Chưa nằm trong phạm vi triển khai hiện tại |

### 2.4. Mục tiêu chức năng

| Mã | Mục tiêu chức năng | Mô tả |
|---|---|---|
| F1 | Lấy dữ liệu quán ăn theo ngôn ngữ | API trả dữ liệu quán ăn đã được chọn translation phù hợp hoặc fallback |
| F2 | Trả danh sách ngôn ngữ khả dụng | Mobile có thể tải danh sách ngôn ngữ từ backend |
| F3 | Gửi và lưu playback log | Mỗi lần phát audio, app gửi log để backend lưu lại |
| F4 | Cung cấp dữ liệu tour | API trả thông tin tour và các điểm trong tour |
| F5 | Quản lý quán ăn trên Web Admin | Tạo, sửa, xóa, lọc và cập nhật translation cho quán |
| F6 | Quản lý tour trên Web Admin | Tạo, sửa, xóa, xem chi tiết và sắp xếp thứ tự các điểm trong tour |
| F7 | Quản lý tài khoản quản trị | Admin tạo người dùng và gán vai trò |
| F8 | Theo dõi dashboard và playback logs | Web Admin hỗ trợ giám sát dữ liệu vận hành |

### 2.5. Mục tiêu phi chức năng

| Mã | Mục tiêu phi chức năng | Mô tả |
|---|---|---|
| N1 | Tính ổn định | Hệ thống phải hoạt động ổn định với quy mô dữ liệu hiện tại |
| N2 | Tính nhất quán dữ liệu | Domain data dùng chung giữa API và Web Admin |
| N3 | Khả năng mở rộng | Mô hình dữ liệu hỗ trợ mở rộng thêm ngôn ngữ, quán và tour |
| N4 | Tính bảo trì | Phân tách rõ app, API, admin và database |
| N5 | Tính phân quyền | Chức năng trên Web Admin được giới hạn theo role |
| N6 | Tương thích với mobile app hiện tại | Tránh thay đổi API quá mạnh gây ảnh hưởng ứng dụng đang phát triển |

### 2.6. Giả định triển khai

| Mã | Giả định |
|---|---|
| A1 | Mobile app là nơi xử lý phần lớn logic runtime liên quan GPS |
| A2 | Backend chủ yếu đóng vai trò cung cấp dữ liệu và lưu log |
| A3 | Nội dung đa ngôn ngữ hiện tập trung tối thiểu vào tiếng Việt và tiếng Anh |
| A4 | Web Admin là công cụ quản trị nội bộ, không phục vụ người dùng cuối |
| A5 | Dữ liệu domain hiện được lưu bằng SQLite và phù hợp với phạm vi đồ án |

---

## 3. Tổng quan hệ thống

### 3.1. Kiến trúc tổng thể

Hệ thống được xây dựng theo mô hình client-server nhiều thành phần. Mobile app đóng vai trò ứng dụng phía người dùng, backend API đóng vai trò lớp truy xuất và xử lý dữ liệu trung gian, SQLite là tầng lưu trữ, còn Web Admin là công cụ quản trị nội dung và dữ liệu vận hành.

Luồng tổng quát của hệ thống như sau:

- Mobile app gọi API để lấy danh sách quán ăn, ngôn ngữ và tour.
- Mobile app sử dụng GPS để xác định vị trí người dùng.
- Khi người dùng đi vào vùng kích hoạt của một quán ăn, app sẽ phát nội dung audio tương ứng.
- Sau khi phát, app gửi playback log về backend.
- Web Admin sử dụng cùng domain database để quản lý nội dung và theo dõi dữ liệu.

### 3.2. Thành phần hệ thống

| Thành phần | Công nghệ | Trách nhiệm chính |
|---|---|---|
| Mobile App | .NET MAUI | Hiển thị dữ liệu, xử lý GPS, phát audio, gửi playback log |
| Backend API | ASP.NET Core Web API | Trả dữ liệu cho app, xử lý logic truy xuất, lưu playback log |
| Domain Database | SQLite | Lưu dữ liệu quán ăn, translation, ngôn ngữ, tour, playback log |
| Web Admin | ASP.NET Core MVC | Quản trị dữ liệu nghiệp vụ, dashboard, user management |
| Auth Database | SQLite + ASP.NET Core Identity | Lưu tài khoản và role của người dùng quản trị |

### 3.3. Mô hình chia tách dữ liệu

Hệ thống hiện sử dụng hai nhóm dữ liệu chính:

| Nhóm dữ liệu | Database | Mục đích |
|---|---|---|
| Domain data | `audio_guide.db` | Lưu dữ liệu nghiệp vụ dùng chung giữa API và Web Admin |
| Authentication data | `admin_auth.db` | Lưu tài khoản đăng nhập và phân quyền cho Web Admin |

Thiết kế này giúp tách dữ liệu nghiệp vụ khỏi dữ liệu xác thực, đồng thời cho phép Web Admin chia sẻ cùng nguồn dữ liệu nghiệp vụ với Backend API.

### 3.4. Cấu trúc domain chính

Các thực thể dữ liệu chính trong phiên bản hiện tại gồm:

| Nhóm | Thực thể |
|---|---|
| Địa điểm | `FoodStall`, `FoodStallTranslation` |
| Ngôn ngữ | `Language` |
| Tour | `Tour`, `TourItem`, `TourTranslation` |
| Theo dõi phát audio | `PlaybackLog` |
| Quản trị người dùng | `ApplicationUser`, `IdentityRole` |

### 3.5. Đặc điểm kiến trúc hiện tại

#### 3.5.1. API định hướng mobile

Backend API hiện được thiết kế theo hướng phục vụ mobile app, trong đó các endpoint chính tập trung vào việc:

- trả danh sách quán ăn đang hoạt động,
- hỗ trợ chọn ngôn ngữ và fallback theo default language,
- trả danh sách ngôn ngữ khả dụng,
- trả danh sách tour đang hoạt động,
- nhận playback log từ ứng dụng.

#### 3.5.2. Domain hỗ trợ đa ngôn ngữ

Hệ thống đang áp dụng mô hình tách entity gốc và entity translation:

- `FoodStall` lưu dữ liệu vị trí và vận hành,
- `FoodStallTranslation` lưu nội dung theo ngôn ngữ,
- `Tour` lưu dữ liệu tour ở mức gốc,
- `TourTranslation` lưu tên và mô tả tour theo ngôn ngữ.

Thiết kế này giúp mở rộng thêm ngôn ngữ mà không làm thay đổi cấu trúc dữ liệu chính.

#### 3.5.3. Web Admin dùng chung domain data

Web Admin không có bản sao dữ liệu nghiệp vụ riêng. Thay vào đó, nó truy cập trực tiếp vào cùng database domain với API để:

- quản lý quán ăn,
- quản lý translation,
- quản lý tour,
- xem playback logs,
- hiển thị dashboard.

Điều này giúp giảm trùng lặp dữ liệu và đảm bảo tính nhất quán giữa phần quản trị và phần phục vụ mobile app.

### 3.6. Kiến trúc triển khai logic

| Khu vực | Logic chính hiện tại |
|---|---|
| Mobile App | GPS trigger, chọn POI phù hợp, phát audio, gửi log |
| Backend API | Truy xuất dữ liệu, chọn translation phù hợp, fallback ngôn ngữ, lưu playback log |
| Web Admin | CRUD dữ liệu, phân quyền truy cập, dashboard tổng quan |
| Database | Lưu trữ dữ liệu và ràng buộc quan hệ giữa các thực thể |

### 3.7. Minh họa cấu trúc hệ thống và luồng dữ liệu
### 3.7.1. Sơ đồ cấu trúc hệ thống
![Sơ đồ cấu trúc hệ thống](./images/3.7.1.png)


### 3.7.2. Sơ đồ luồng dữ liệu mức tổng quan

![Sơ đồ luồng dữ liệu](./images/3.7.2.png)

---

## 4. Actors và phân quyền

### 4.1. Actors chính

| Actor | Mô tả |
|---|---|
| User | Người dùng cuối sử dụng mobile app để khám phá địa điểm |
| Admin | Người quản trị hệ thống trên Web Admin, có toàn quyền quản lý |
| FoodStallOwner | Người dùng quản trị với quyền hạn giới hạn, chủ yếu xem dữ liệu |

### 4.2. Quyền truy cập mức cao

| Chức năng | User | Admin | FoodStallOwner |
|---|---:|---:|---:|
| Xem danh sách quán trên mobile | Có | Không áp dụng | Không áp dụng |
| Nhận audio tự động theo GPS | Có | Không áp dụng | Không áp dụng |
| Xem danh sách ngôn ngữ | Có | Không áp dụng | Không áp dụng |
| Tham gia tour | Có | Không áp dụng | Không áp dụng |
| Xem dashboard | Không | Có | Có |
| Xem danh sách quán trên Web Admin | Không | Có | Có |
| Tạo quán mới | Không | Có | Không |
| Sửa quán | Không | Có | Không |
| Xóa quán | Không | Có | Không |
| Cập nhật translation quán | Không | Có | Không |
| Xem danh sách tour | Không | Có | Có |
| Xem chi tiết tour | Không | Có | Có |
| Tạo tour | Không | Có | Không |
| Sửa tour | Không | Có | Không |
| Xóa tour | Không | Có | Không |
| Xem playback logs | Không | Có | Có |
| Quản lý người dùng admin | Không | Có | Không |

### 4.3. Ghi chú phân quyền hiện tại

Hệ thống Web Admin hiện sử dụng `ASP.NET Core Identity` kết hợp role-based authorization. Hai vai trò hiện có trong hệ thống là:

- `Admin`
- `FoodStallOwner`

Trong đó:

- `Admin` có quyền thực hiện các thao tác tạo, cập nhật, xóa và quản lý user.
- `FoodStallOwner` có quyền truy cập các màn hình xem dữ liệu như dashboard, danh sách quán, danh sách tour và playback logs, nhưng không có quyền chỉnh sửa dữ liệu.

**[TODO - Chèn UML Use Case Diagram cho phân quyền hệ thống]**

---
## 5. Luồng nghiệp vụ chính

Phần này mô tả các luồng nghiệp vụ cốt lõi của hệ thống ở mức sản phẩm và hệ thống. Các luồng được viết theo hướng bám sát kiến trúc hiện tại: mobile app chịu trách nhiệm xử lý trải nghiệm thời gian thực, backend API chịu trách nhiệm cung cấp dữ liệu và ghi nhận log, còn Web Admin phục vụ quản trị và giám sát.

### 5.1. GPS Trigger Flow

#### 5.1.1. Mục tiêu

GPS Trigger Flow là luồng nghiệp vụ quan trọng nhất của hệ thống. Mục tiêu của luồng này là phát hiện khi người dùng di chuyển vào vùng lân cận của một địa điểm và tự động kích hoạt nội dung thuyết minh phù hợp.

Luồng này là nền tảng tạo ra giá trị trải nghiệm chính của sản phẩm, vì nó giúp người dùng nghe nội dung giới thiệu theo bối cảnh vị trí thực tế mà không cần thao tác thủ công nhiều lần.

#### 5.1.2. Tiền điều kiện

| Mã | Điều kiện |
|---|---|
| P1 | Mobile app đã được cài đặt và có thể truy cập GPS |
| P2 | Người dùng đã cấp quyền truy cập vị trí |
| P3 | App đã tải danh sách các địa điểm từ backend API |
| P4 | Mỗi địa điểm có đầy đủ thông tin tọa độ, bán kính kích hoạt và nội dung cần phát |
| P5 | App đang ở trạng thái có thể theo dõi vị trí người dùng |

#### 5.1.3. Dữ liệu đầu vào

| Dữ liệu | Nguồn | Mô tả |
|---|---|---|
| Vị trí hiện tại của người dùng | Mobile GPS | Kinh độ, vĩ độ tại thời điểm kiểm tra |
| Danh sách địa điểm đang hoạt động | Backend API | Tập các `FoodStall` mà `IsActive = true` |
| Bán kính kích hoạt | Domain data | Thuộc tính `Radius` của từng địa điểm |
| Độ ưu tiên | Domain data | Thuộc tính `Priority` dùng để sắp thứ tự xử lý |
| Nội dung theo ngôn ngữ | Translation data | `FoodStallTranslation` theo ngôn ngữ được chọn hoặc fallback |

#### 5.1.4. Mô tả luồng chính

| Bước | Mô tả |
|---|---|
| 1 | Mobile app định kỳ lấy vị trí GPS hiện tại của người dùng |
| 2 | App so sánh vị trí hiện tại với danh sách các địa điểm đã tải từ API |
| 3 | Với mỗi địa điểm, app tính khoảng cách từ người dùng đến tọa độ của địa điểm |
| 4 | App xác định các địa điểm mà khoảng cách hiện tại nhỏ hơn hoặc bằng `Radius` |
| 5 | Nếu có nhiều địa điểm thỏa điều kiện, app chọn địa điểm phù hợp theo quy tắc ưu tiên |
| 6 | App xác định nội dung audio tương ứng với địa điểm đã chọn |
| 7 | App kích hoạt phát audio cho địa điểm đó |
| 8 | Sau khi phát hoặc bắt đầu phát, app tiếp tục luồng ghi nhận playback log |

#### 5.1.5. Kết quả đầu ra

| Kết quả | Mô tả |
|---|---|
| Audio được kích hoạt | Nội dung thuyết minh của địa điểm được phát cho người dùng |
| Địa điểm được xác định | Hệ thống xác định được POI phù hợp tại thời điểm người dùng tiến vào vùng kích hoạt |
| Luồng playback được nối tiếp | Sau khi phát audio, app có thể gửi log về backend |

#### 5.1.6. Quy tắc nghiệp vụ liên quan

| Mã | Quy tắc |
|---|---|
| R1 | Chỉ các địa điểm đang hoạt động mới được dùng trong luồng GPS Trigger |
| R2 | Mỗi địa điểm phải có dữ liệu vị trí và bán kính kích hoạt |
| R3 | Khi có nhiều địa điểm cùng nằm trong vùng kích hoạt, app cần có quy tắc chọn một địa điểm phù hợp |
| R4 | `Priority` là thuộc tính nghiệp vụ phục vụ việc sắp thứ tự địa điểm trong xử lý |
| R5 | Nội dung phát phải phù hợp với ngôn ngữ hiện tại hoặc ngôn ngữ fallback |
| R6 | Luồng GPS Trigger là trọng tâm của phiên bản hiện tại của hệ thống |

#### 5.1.7. Các tình huống ngoại lệ

| Tình huống | Cách xử lý mong đợi |
|---|---|
| Người dùng chưa cấp quyền GPS | App không thực hiện trigger và yêu cầu cấp quyền |
| Không lấy được vị trí hiện tại | App bỏ qua chu kỳ kiểm tra hiện tại và thử lại sau |
| Không có địa điểm nào trong bán kính | App không phát audio |
| Địa điểm có dữ liệu translation không đầy đủ | App dùng dữ liệu fallback nếu có |
| Audio không thể phát | App ghi nhận trạng thái lỗi ở mức ứng dụng nếu cần |

**[TODO - Chèn sơ đồ minh họa GPS Trigger Flow]**

Gợi ý sơ đồ minh họa:
- User position update
- Compare distance to FoodStalls
- Filter by radius
- Resolve candidate POI
- Select by priority
- Trigger audio

**[TODO - Chèn UML Activity Diagram cho GPS Trigger Flow]**

---

### 5.2. Playback Flow

#### 5.2.1. Mục tiêu

Playback Flow mô tả quá trình phát nội dung audio cho người dùng và ghi nhận lịch sử phát về backend. Đây là luồng giúp kết nối trải nghiệm người dùng ở mobile app với dữ liệu giám sát và thống kê ở hệ quản trị.

Luồng này không chỉ phục vụ việc phát nội dung mà còn tạo ra dữ liệu vận hành để dashboard và Web Admin có thể theo dõi mức độ sử dụng hệ thống.

#### 5.2.2. Tiền điều kiện

| Mã | Điều kiện |
|---|---|
| P1 | App đã xác định được địa điểm phù hợp để phát |
| P2 | App có dữ liệu audio hoặc thông tin cần phát cho địa điểm đó |
| P3 | Backend API đang hoạt động và có thể nhận request ghi log |
| P4 | Địa điểm được phát tồn tại hợp lệ trong domain data |

#### 5.2.3. Dữ liệu đầu vào

| Dữ liệu | Nguồn | Mô tả |
|---|---|---|
| `FoodStallId` | Mobile app | Mã địa điểm được phát |
| `LanguageCode` | Mobile app | Ngôn ngữ đang sử dụng khi phát |
| `TriggerType` | Mobile app | Loại kích hoạt, trong phiên bản hiện tại tập trung vào GPS |
| `DurationSeconds` | Mobile app | Thời lượng phát hoặc thời lượng ghi nhận |
| `PlayedAt` | Backend API | Thời điểm được backend ghi nhận bằng thời gian hệ thống |

#### 5.2.4. Mô tả luồng chính

| Bước | Mô tả |
|---|---|
| 1 | App bắt đầu phát nội dung audio cho địa điểm đã chọn |
| 2 | App xác định dữ liệu log cần gửi về backend |
| 3 | App gửi request `POST /api/PlaybackLogs` |
| 4 | Backend API nhận dữ liệu request |
| 5 | Backend tạo một bản ghi `PlaybackLog` mới |
| 6 | Backend tự gán `PlayedAt` bằng thời gian UTC hiện tại |
| 7 | Backend lưu log vào cơ sở dữ liệu |
| 8 | Backend trả kết quả thành công cho app |
| 9 | Dữ liệu log sau đó có thể được dùng trong dashboard và màn hình playback logs của Web Admin |

#### 5.2.5. Kết quả đầu ra

| Kết quả | Mô tả |
|---|---|
| PlaybackLog được lưu | Hệ thống có bản ghi về lần phát audio |
| Dashboard có dữ liệu cập nhật | Số liệu thống kê và top địa điểm có thể được tính từ log |
| Web Admin có thể tra cứu | Quản trị viên có thể xem lịch sử phát theo bộ lọc |

#### 5.2.6. Quy tắc nghiệp vụ liên quan

| Mã | Quy tắc |
|---|---|
| R1 | Mỗi bản ghi log phải gắn với một `FoodStallId` hợp lệ |
| R2 | `PlayedAt` được backend ghi nhận để tránh phụ thuộc giờ thiết bị |
| R3 | `LanguageCode` phản ánh ngôn ngữ thực tế được sử dụng khi phát |
| R4 | `TriggerType` phục vụ phân loại nguồn phát trong dữ liệu vận hành |
| R5 | Playback log là nguồn dữ liệu đầu vào cho dashboard và thống kê top địa điểm |

#### 5.2.7. Các tình huống ngoại lệ

| Tình huống | Cách xử lý mong đợi |
|---|---|
| Request gửi lên không hợp lệ | Backend trả `400 Bad Request` |
| Có lỗi khi lưu dữ liệu | Backend trả `500 Internal Server Error` |
| App phát audio nhưng gửi log thất bại | Trải nghiệm nghe không nên bị chặn; lỗi log được xem là lỗi phụ trợ |
| `FoodStallId` không hợp lệ | Backend nên từ chối hoặc ghi nhận lỗi theo chính sách triển khai |

**[TODO - Chèn sơ đồ minh họa Playback Flow]**

Gợi ý sơ đồ minh họa:
- App plays audio
- App posts log
- API creates PlaybackLog
- Database persists log
- Admin dashboard reads aggregated data

**[TODO - Chèn UML Sequence Diagram cho Playback Flow]**

---

### 5.3. Tour Flow

#### 5.3.1. Mục tiêu

Tour Flow mô tả quá trình người dùng tham gia một tour gồm nhiều điểm dừng và hệ thống hỗ trợ phát nội dung theo danh sách điểm của tour. Luồng này mở rộng trải nghiệm từ mô hình khám phá tự do sang mô hình khám phá có lộ trình.

#### 5.3.2. Tiền điều kiện

| Mã | Điều kiện |
|---|---|
| P1 | Hệ thống đã có dữ liệu tour đang hoạt động |
| P2 | Mỗi tour có ít nhất một `TourItem` hợp lệ |
| P3 | Các `TourItem` được sắp theo `OrderIndex` |
| P4 | App có thể tải dữ liệu tour từ backend |
| P5 | Người dùng đã chọn một tour để tham gia |

#### 5.3.3. Dữ liệu đầu vào

| Dữ liệu | Nguồn | Mô tả |
|---|---|---|
| Danh sách tour active | Backend API | Từ endpoint `GET /api/Tours` |
| Bản dịch tour | Domain data | `TourTranslation` |
| Danh sách điểm trong tour | Domain data | `TourItem` sắp theo `OrderIndex` |
| Thông tin địa điểm của từng điểm dừng | Domain data | `FoodStall` và `FoodStallTranslation` |

#### 5.3.4. Mô tả luồng chính

| Bước | Mô tả |
|---|---|
| 1 | Người dùng truy cập chức năng tour trong mobile app |
| 2 | App tải danh sách các tour đang hoạt động từ backend |
| 3 | Người dùng chọn một tour |
| 4 | App tải hoặc giữ trong bộ nhớ danh sách các điểm dừng thuộc tour đó |
| 5 | App sắp thứ tự các điểm theo `OrderIndex` |
| 6 | Trong quá trình di chuyển, app sử dụng dữ liệu tour kết hợp vị trí GPS để xác định điểm dừng phù hợp |
| 7 | Khi người dùng đến gần một điểm trong tour, app kích hoạt audio cho điểm đó |
| 8 | Sau mỗi lần phát, app tiếp tục gửi playback log về backend |
| 9 | App tiếp tục theo dõi cho đến khi người dùng hoàn thành tour hoặc rời khỏi luồng tour |

#### 5.3.5. Kết quả đầu ra

| Kết quả | Mô tả |
|---|---|
| Người dùng tham gia tour | Hệ thống hỗ trợ trải nghiệm theo lộ trình nhiều điểm |
| Các điểm được phát theo danh sách tour | App có cơ sở dữ liệu để điều hướng theo thứ tự |
| Dữ liệu phát tiếp tục được ghi log | Hệ thống vẫn theo dõi usage như luồng playback thông thường |

#### 5.3.6. Quy tắc nghiệp vụ liên quan

| Mã | Quy tắc |
|---|---|
| R1 | Chỉ các tour có `IsActive = true` mới được dùng cho mobile app |
| R2 | Một tour có thể có nhiều bản dịch, mỗi ngôn ngữ tối đa một bản dịch |
| R3 | Một địa điểm không được xuất hiện lặp lại trong cùng một tour |
| R4 | `OrderIndex` quyết định thứ tự các điểm trong tour |
| R5 | Tour phải có ít nhất một điểm dừng hợp lệ để có ý nghĩa sử dụng |
| R6 | Tên và mô tả tour cần được hỗ trợ đa ngôn ngữ tương tự nội dung địa điểm |

#### 5.3.7. Các tình huống ngoại lệ

| Tình huống | Cách xử lý mong đợi |
|---|---|
| Tour không có điểm dừng hợp lệ | Không nên cho phép sử dụng tour đó |
| Dữ liệu translation của tour không đầy đủ | App hoặc backend cần dùng dữ liệu fallback phù hợp |
| Điểm dừng trong tour bị inactive hoặc dữ liệu không nhất quán | Cần xử lý trong tầng quản trị hoặc validation dữ liệu |
| App không thể tải danh sách tour | Chức năng tour không hoạt động nhưng các chức năng khác của app vẫn có thể tiếp tục |

**[TODO - Chèn sơ đồ minh họa Tour Flow]**

Gợi ý sơ đồ minh họa:
- Load tours
- User selects tour
- Load ordered stops
- GPS checks current stop
- Trigger audio for stop
- Send playback log

**[TODO - Chèn UML Activity Diagram hoặc Sequence Diagram cho Tour Flow]**

---

### 5.4. Data Retrieval Flow cho Mobile App

#### 5.4.1. Mục tiêu

Luồng này mô tả cách mobile app tải dữ liệu nền từ backend để phục vụ các chức năng GPS Trigger, playback và tour. Đây là luồng hỗ trợ nền, nhưng có vai trò quyết định trong việc app có đủ dữ liệu để hoạt động chính xác hay không.

#### 5.4.2. Mô tả luồng chính

| Bước | Mô tả |
|---|---|
| 1 | App khởi động hoặc vào màn hình chức năng cần dữ liệu |
| 2 | App gọi `GET /api/Languages` để lấy danh sách ngôn ngữ hỗ trợ |
| 3 | App xác định ngôn ngữ đang dùng |
| 4 | App gọi `GET /api/FoodStalls?lang=...` để lấy danh sách địa điểm theo ngôn ngữ |
| 5 | Backend chọn translation phù hợp theo ngôn ngữ yêu cầu hoặc fallback sang default language |
| 6 | App nhận dữ liệu `FoodStallMobileDto` để dùng cho danh sách, bản đồ và GPS trigger |
| 7 | Nếu người dùng vào chức năng tour, app gọi `GET /api/Tours` để lấy dữ liệu tour active |

#### 5.4.3. Quy tắc nghiệp vụ liên quan

| Mã | Quy tắc |
|---|---|
| R1 | Dữ liệu trả cho mobile chỉ bao gồm các `FoodStall` đang active |
| R2 | API food stall hỗ trợ query `lang` để chọn bản dịch |
| R3 | Nếu không có translation theo ngôn ngữ yêu cầu, backend fallback sang default language |
| R4 | API ngôn ngữ trả danh sách ngôn ngữ hỗ trợ kèm thông tin hiển thị |
| R5 | API tours hiện trả các tour đang active |

**[TODO - Chèn sơ đồ minh họa Data Retrieval Flow]**

Gợi ý sơ đồ minh họa:
- App requests languages
- App selects active language
- App requests food stalls by lang
- API resolves translation
- App requests tours if needed

**[TODO - Chèn UML Sequence Diagram cho Mobile Data Retrieval]**

---

### 5.5. Admin Management Flow

#### 5.5.1. Mục tiêu

Admin Management Flow mô tả cách Web Admin quản lý dữ liệu nghiệp vụ của hệ thống. Luồng này phục vụ vận hành nội dung và bảo đảm dữ liệu được duy trì nhất quán để mobile app có thể sử dụng.

#### 5.5.2. Nhóm chức năng quản trị chính

| Nhóm chức năng | Mô tả |
|---|---|
| Quản lý quán ăn | Tạo, sửa, xóa, lọc, tìm kiếm và cập nhật translation cho quán |
| Quản lý tour | Tạo, sửa, xóa, xem chi tiết tour và sắp thứ tự điểm dừng |
| Quản lý playback logs | Xem lịch sử phát theo bộ lọc |
| Dashboard | Theo dõi số liệu tổng quan và recent activity |
| Quản lý user admin | Tạo người dùng và gán role |

#### 5.5.3. Mô tả luồng quản lý quán ăn

| Bước | Mô tả |
|---|---|
| 1 | Admin đăng nhập vào Web Admin |
| 2 | Admin truy cập danh sách quán ăn |
| 3 | Admin có thể tìm kiếm theo tên hoặc địa chỉ, lọc theo trạng thái active/inactive |
| 4 | Khi tạo quán mới, admin nhập dữ liệu vị trí và thông tin vận hành cơ bản |
| 5 | Sau khi lưu quán, admin tiếp tục cập nhật translation cho tiếng Việt và tiếng Anh |
| 6 | Dữ liệu sau khi lưu sẽ được dùng chung cho API và mobile app |

#### 5.5.4. Mô tả luồng quản lý tour

| Bước | Mô tả |
|---|---|
| 1 | Admin truy cập danh sách tour |
| 2 | Admin tạo hoặc chỉnh sửa tour |
| 3 | Admin nhập thông tin dịch cho tiếng Việt và tiếng Anh |
| 4 | Admin chọn các quán thuộc tour |
| 5 | Admin sắp thứ tự các điểm bằng `OrderIndex` |
| 6 | Sau khi lưu, tour có thể được mobile app truy xuất nếu đang active |

#### 5.5.5. Mô tả luồng quản lý người dùng

| Bước | Mô tả |
|---|---|
| 1 | Admin truy cập màn hình người dùng |
| 2 | Admin tạo tài khoản mới |
| 3 | Admin gán role là `Admin` hoặc `FoodStallOwner` |
| 4 | Người dùng mới có thể đăng nhập Web Admin với quyền tương ứng |

#### 5.5.6. Quy tắc nghiệp vụ liên quan

| Mã | Quy tắc |
|---|---|
| R1 | Chỉ `Admin` mới được tạo, sửa, xóa dữ liệu nghiệp vụ |
| R2 | `FoodStallOwner` chỉ có quyền xem các màn hình được cho phép |
| R3 | Translation của quán và tour hiện được quản lý tối thiểu cho tiếng Việt và tiếng Anh |
| R4 | Dữ liệu quản trị cập nhật trực tiếp vào domain database dùng chung với API |
| R5 | Auth data của Web Admin được lưu riêng với domain data |

**[TODO - Chèn sơ đồ minh họa Admin Management Flow]**

Gợi ý sơ đồ minh họa:
- Admin login
- Manage FoodStalls
- Edit translations
- Manage tours
- View logs/dashboard
- Manage users

**[TODO - Chèn UML Use Case Diagram cho Web Admin]**

---

### 5.6. Tổng kết các luồng nghiệp vụ cốt lõi

| Luồng | Vai trò | Thành phần tham gia chính |
|---|---|---|
| GPS Trigger Flow | Kích hoạt nội dung theo vị trí | Mobile App, Domain data |
| Playback Flow | Ghi nhận lần phát audio | Mobile App, Backend API, Domain Database |
| Tour Flow | Hỗ trợ trải nghiệm theo lộ trình | Mobile App, Backend API, Domain data |
| Data Retrieval Flow | Tải dữ liệu nền cho app | Mobile App, Backend API |
| Admin Management Flow | Quản trị và vận hành hệ thống | Web Admin, Domain Database, Auth Database |

Các luồng trên tạo thành xương sống nghiệp vụ của hệ thống. Trong đó, GPS Trigger Flow và Playback Flow là hai luồng trọng tâm của giá trị sản phẩm hiện tại, còn Tour Flow và Admin Management Flow đóng vai trò mở rộng trải nghiệm và duy trì vận hành nội dung.

---

## 6. Yêu cầu chức năng (Functional Requirements)

Phần này mô tả các chức năng mà hệ thống phải cung cấp. Các yêu cầu được phân tách theo từng thành phần chính trong kiến trúc: Mobile App, Backend API và Web Admin.

Các yêu cầu được xây dựng dựa trên hệ thống hiện tại và bám sát implementation trong code.

---

### 6.1. Mobile App

Mobile App là thành phần trực tiếp tương tác với người dùng, chịu trách nhiệm hiển thị dữ liệu, xử lý GPS và phát audio.

#### 6.1.1. Quản lý dữ liệu địa điểm

| Mã | Chức năng | Mô tả |
|---|---|---|
| M-F1 | Tải danh sách địa điểm | App gọi API để lấy danh sách `FoodStall` đang hoạt động |
| M-F2 | Hiển thị thông tin địa điểm | Hiển thị tên, địa chỉ, mô tả, specialty, giá |
| M-F3 | Hiển thị vị trí trên bản đồ | Sử dụng latitude/longitude để hiển thị POI |
| M-F4 | Hiển thị hình ảnh | Hiển thị ảnh từ `ImageUrl` |
| M-F5 | Hỗ trợ link bản đồ | Cho phép mở `MapLink` nếu có |

---

#### 6.1.2. Xử lý GPS và trigger

| Mã | Chức năng | Mô tả |
|---|---|---|
| M-F6 | Lấy vị trí người dùng | Sử dụng GPS để lấy tọa độ hiện tại |
| M-F7 | Tính khoảng cách tới POI | So sánh vị trí user với danh sách POI |
| M-F8 | Xác định vùng kích hoạt | So sánh khoảng cách với `Radius` |
| M-F9 | Chọn POI phù hợp | Chọn POI theo `Priority` nếu có nhiều điểm |
| M-F10 | Kích hoạt audio tự động | Khi vào vùng kích hoạt, tự động phát audio |

---

#### 6.1.3. Playback audio

| Mã | Chức năng | Mô tả |
|---|---|---|
| M-F11 | Phát audio | Phát nội dung từ `AudioUrl` hoặc TTS |
| M-F12 | Quản lý trạng thái playback | Theo dõi đang phát, dừng hoặc hoàn thành |
| M-F13 | Tránh phát trùng | Không phát lại liên tục cùng một POI trong thời gian ngắn |

---

#### 6.1.4. Gửi playback log

| Mã | Chức năng | Mô tả |
|---|---|---|
| M-F14 | Tạo dữ liệu log | Chuẩn bị `FoodStallId`, `LanguageCode`, `TriggerType`, `DurationSeconds` |
| M-F15 | Gửi log về backend | Gọi API `POST /api/PlaybackLogs` |
| M-F16 | Xử lý lỗi gửi log | Không làm gián đoạn trải nghiệm nếu gửi log thất bại |

---

#### 6.1.5. Quản lý ngôn ngữ

| Mã | Chức năng | Mô tả |
|---|---|---|
| M-F17 | Tải danh sách ngôn ngữ | Gọi API `/api/Languages` |
| M-F18 | Chọn ngôn ngữ | Người dùng chọn ngôn ngữ hiển thị |
| M-F19 | Áp dụng ngôn ngữ | Gửi `lang` lên API khi lấy dữ liệu |
| M-F20 | Fallback ngôn ngữ | Sử dụng default language nếu thiếu translation |

---

#### 6.1.6. Tour

| Mã | Chức năng | Mô tả |
|---|---|---|
| M-F21 | Tải danh sách tour | Gọi API `/api/Tours` |
| M-F22 | Hiển thị tour | Hiển thị tên và mô tả tour |
| M-F23 | Tham gia tour | Người dùng chọn tour |
| M-F24 | Điều hướng theo tour | App sử dụng danh sách `TourItem` |
| M-F25 | Phát audio theo tour | Trigger audio theo từng điểm |

---

### 6.2. Backend API

Backend API chịu trách nhiệm cung cấp dữ liệu và ghi nhận hành vi người dùng.

#### 6.2.1. FoodStall API

| Mã | Endpoint | Mô tả |
|---|---|---|
| B-F1 | GET /api/FoodStalls | Trả danh sách địa điểm |
| B-F2 | GET /api/FoodStalls/{id} | Trả chi tiết một địa điểm |
| B-F3 | Lọc theo trạng thái | Chỉ trả `IsActive = true` |
| B-F4 | Hỗ trợ đa ngôn ngữ | Nhận query `lang` |
| B-F5 | Fallback translation | Dùng default language nếu thiếu |
| B-F6 | Sắp xếp theo priority | Order theo `Priority` |
| B-F7 | Trả DTO cho mobile | Sử dụng `FoodStallMobileDto` |

---

#### 6.2.2. Language API

| Mã | Endpoint | Mô tả |
|---|---|---|
| B-F8 | GET /api/Languages | Trả danh sách ngôn ngữ |
| B-F9 | Xác định default language | Dựa vào `IsDefault` |

---

#### 6.2.3. PlaybackLog API

| Mã | Endpoint | Mô tả |
|---|---|---|
| B-F10 | POST /api/PlaybackLogs | Nhận log từ app |
| B-F11 | Lưu playback log | Tạo bản ghi `PlaybackLog` |
| B-F12 | Ghi thời gian server | Sử dụng `DateTime.UtcNow` |
| B-F13 | Xử lý lỗi | Trả lỗi nếu request không hợp lệ |

---

#### 6.2.4. Tour API

| Mã | Endpoint | Mô tả |
|---|---|---|
| B-F14 | GET /api/Tours | Trả danh sách tour |
| B-F15 | Lọc tour active | Chỉ trả `IsActive = true` |
| B-F16 | Bao gồm tour items | Trả danh sách điểm |
| B-F17 | Bao gồm translation | Trả dữ liệu đa ngôn ngữ |

---

### 6.3. Web Admin

Web Admin là công cụ quản trị dữ liệu và giám sát hệ thống.

---

#### 6.3.1. Xác thực và phân quyền

| Mã | Chức năng | Mô tả |
|---|---|---|
| A-F1 | Đăng nhập | Người dùng đăng nhập bằng email/password |
| A-F2 | Đăng xuất | Kết thúc phiên làm việc |
| A-F3 | Phân quyền theo role | `Admin`, `FoodStallOwner` |
| A-F4 | Kiểm soát truy cập | Sử dụng `[Authorize]` |

---

#### 6.3.2. Quản lý FoodStall

| Mã | Chức năng | Mô tả |
|---|---|---|
| A-F5 | Xem danh sách quán | Có tìm kiếm và filter |
| A-F6 | Tạo quán | Nhập thông tin vị trí và metadata |
| A-F7 | Sửa quán | Cập nhật dữ liệu |
| A-F8 | Xóa quán | Xóa khỏi hệ thống |
| A-F9 | Quản lý translation | Edit tiếng Việt và tiếng Anh |

---

#### 6.3.3. Quản lý Tour

| Mã | Chức năng | Mô tả |
|---|---|---|
| A-F10 | Xem danh sách tour | Có tìm kiếm |
| A-F11 | Tạo tour | Nhập translation và danh sách điểm |
| A-F12 | Sửa tour | Cập nhật thông tin |
| A-F13 | Xóa tour | Xóa khỏi hệ thống |
| A-F14 | Sắp xếp điểm | Dùng `OrderIndex` |

---

#### 6.3.4. Playback Logs

| Mã | Chức năng | Mô tả |
|---|---|---|
| A-F15 | Xem danh sách log | Hiển thị lịch sử phát |
| A-F16 | Lọc log | Theo language, triggerType, foodStall |
| A-F17 | Xem chi tiết | Bao gồm thời gian và duration |

---

#### 6.3.5. Dashboard

| Mã | Chức năng | Mô tả |
|---|---|---|
| A-F18 | Hiển thị tổng số quán | Tổng và active |
| A-F19 | Hiển thị tổng log | Playback logs |
| A-F20 | Thống kê theo trigger | GPS |
| A-F21 | Thống kê theo ngôn ngữ | vi, en |
| A-F22 | Recent logs | 5 log gần nhất |
| A-F23 | Top food stalls | Theo số lượt phát |

---

#### 6.3.6. Quản lý người dùng

| Mã | Chức năng | Mô tả |
|---|---|---|
| A-F24 | Xem danh sách user | Hiển thị email và role |
| A-F25 | Tạo user | Tạo tài khoản mới |
| A-F26 | Gán role | Admin hoặc FoodStallOwner |
| A-F27 | Validate dữ liệu | Kiểm tra email trùng, role hợp lệ |

---

### 6.4. Tổng kết

| Thành phần | Số lượng chức năng |
|---|---|
| Mobile App | 25 |
| Backend API | 17 |
| Web Admin | 27 |

Các yêu cầu chức năng trên phản ánh đầy đủ các khả năng hiện tại của hệ thống và đóng vai trò làm cơ sở để triển khai, kiểm thử và đánh giá hệ thống.

---

## 7. Quy tắc nghiệp vụ (Business Rules)

Phần này mô tả các quy tắc nghiệp vụ cốt lõi chi phối hành vi của hệ thống. Các quy tắc được rút ra từ thiết kế dữ liệu, logic API và cách hệ thống đang được triển khai thực tế.

---

### 7.1. Quy tắc liên quan đến FoodStall

| Mã | Quy tắc | Mô tả |
|---|---|---|
| BR-FS1 | Chỉ hiển thị quán active | Chỉ các `FoodStall` có `IsActive = true` mới được trả về cho mobile app |
| BR-FS2 | Mỗi quán có tọa độ xác định | Mỗi `FoodStall` phải có `Latitude` và `Longitude` |
| BR-FS3 | Mỗi quán có bán kính kích hoạt | `Radius` xác định vùng GPS trigger (đơn vị mét) |
| BR-FS4 | Priority dùng để sắp thứ tự | Khi có nhiều POI gần nhau, `Priority` được dùng để quyết định thứ tự |
| BR-FS5 | Thông tin hiển thị có thể null | Các trường như `ImageUrl`, `Address`, `PriceRange` có thể không bắt buộc |
| BR-FS6 | MapLink là tùy chọn | Nếu có, app có thể dùng để mở bản đồ ngoài |

---

### 7.2. Quy tắc đa ngôn ngữ

| Mã | Quy tắc | Mô tả |
|---|---|---|
| BR-L1 | Mỗi language có mã duy nhất | `LanguageCode` là duy nhất |
| BR-L2 | Có một default language | Hệ thống phải có đúng một language với `IsDefault = true` |
| BR-L3 | Mỗi quán chỉ có một translation cho mỗi language | Unique `(FoodStallId, LanguageId)` |
| BR-L4 | Mỗi tour chỉ có một translation cho mỗi language | Unique `(TourId, LanguageId)` |
| BR-L5 | API hỗ trợ chọn ngôn ngữ | Thông qua query `lang` |
| BR-L6 | Có fallback ngôn ngữ | Nếu không có translation theo `lang`, dùng default language |
| BR-L7 | Language dùng cho UI | `DisplayName`, `FlagIcon`, `IntroText` phục vụ hiển thị |

---

### 7.3. Quy tắc liên quan đến Translation

| Mã | Quy tắc | Mô tả |
|---|---|---|
| BR-T1 | Name là bắt buộc | `Name` trong translation không được null |
| BR-T2 | Description là tùy chọn | Có thể null |
| BR-T3 | Specialty là tùy chọn | Dùng để mô tả món đặc trưng |
| BR-T4 | AudioUrl là tùy chọn | Có thể null nếu dùng TTS |
| BR-T5 | Translation phụ thuộc entity gốc | Translation không tồn tại độc lập |
| BR-T6 | Xóa cascade | Khi xóa `FoodStall`, các translation bị xóa theo |

---

### 7.4. Quy tắc GPS Trigger

| Mã | Quy tắc | Mô tả |
|---|---|---|
| BR-G1 | Trigger dựa trên khoảng cách | So sánh khoảng cách user với `Radius` |
| BR-G2 | Chỉ trigger với quán active | Không trigger quán inactive |
| BR-G3 | Có thể có nhiều POI cùng lúc | Hệ thống phải chọn 1 POI phù hợp |
| BR-G4 | Priority quyết định chọn POI | POI có priority phù hợp sẽ được chọn |
| BR-G5 | Không trigger liên tục | Cần cơ chế tránh phát lặp lại liên tục |
| BR-G6 | GPS là trigger chính | Phiên bản hiện tại không sử dụng QR |

---

### 7.5. Quy tắc Playback

| Mã | Quy tắc | Mô tả |
|---|---|---|
| BR-P1 | Mỗi lần phát tạo 1 log | Mỗi playback tương ứng 1 `PlaybackLog` |
| BR-P2 | Log phải gắn với FoodStall | `FoodStallId` là bắt buộc |
| BR-P3 | Log có ngôn ngữ | `LanguageCode` phản ánh ngôn ngữ đang dùng |
| BR-P4 | Log có trigger type | Hiện tại chủ yếu là `GPS` |
| BR-P5 | Backend ghi thời gian | `PlayedAt` dùng `UTC` từ server |
| BR-P6 | Duration do app cung cấp | Backend không tự tính |
| BR-P7 | Log không được làm gián đoạn app | Nếu log lỗi, app vẫn hoạt động bình thường |

---

### 7.6. Quy tắc Tour

| Mã | Quy tắc | Mô tả |
|---|---|---|
| BR-TO1 | Chỉ dùng tour active | `IsActive = true` |
| BR-TO2 | Tour phải có ít nhất 1 điểm | Không có điểm thì không hợp lệ |
| BR-TO3 | Không trùng quán trong tour | Unique `(TourId, FoodStallId)` |
| BR-TO4 | Có thứ tự điểm | `OrderIndex` xác định thứ tự |
| BR-TO5 | Có translation cho tour | Tương tự FoodStall |
| BR-TO6 | Tour gồm nhiều FoodStall | Quan hệ 1-N qua `TourItem` |

---

### 7.7. Quy tắc Web Admin

#### 7.7.1. Phân quyền

| Mã | Quy tắc | Mô tả |
|---|---|---|
| BR-A1 | Admin toàn quyền | CRUD toàn bộ dữ liệu |
| BR-A2 | FoodStallOwner chỉ xem | Không được chỉnh sửa |
| BR-A3 | Endpoint bảo vệ bằng Authorize | Sử dụng role-based |
| BR-A4 | User phải đăng nhập | Trước khi truy cập hệ thống |

---

#### 7.7.2. Quản lý dữ liệu

| Mã | Quy tắc | Mô tả |
|---|---|---|
| BR-A5 | CRUD FoodStall | Tạo, sửa, xóa |
| BR-A6 | Translation edit riêng | Có màn hình riêng cho translation |
| BR-A7 | Tour quản lý độc lập | Có module riêng |
| BR-A8 | Playback log chỉ đọc | Không chỉnh sửa |
| BR-A9 | Dashboard dùng dữ liệu log | Tính toán từ PlaybackLogs |

---

#### 7.7.3. Quản lý người dùng

| Mã | Quy tắc | Mô tả |
|---|---|---|
| BR-A10 | Email là duy nhất | Không cho tạo trùng |
| BR-A11 | Role phải tồn tại | Validate trước khi gán |
| BR-A12 | User phải có role | Không có role thì không hợp lệ |
| BR-A13 | Password theo policy | Tối thiểu 6 ký tự |

---

### 7.8. Quy tắc dữ liệu và hệ thống

| Mã | Quy tắc | Mô tả |
|---|---|---|
| BR-S1 | Domain DB dùng chung | API và Admin dùng chung DB |
| BR-S2 | Auth DB tách riêng | Identity dùng DB riêng |
| BR-S3 | Dữ liệu seed khi khởi động | Dùng `DbSeeder` |
| BR-S4 | Migration tự động | Chạy khi startup |
| BR-S5 | API không xử lý realtime | Logic GPS nằm ở mobile |

---

### 7.9. Tổng kết

Các Business Rules trên đóng vai trò:

- Định nghĩa hành vi hệ thống
- Đảm bảo tính nhất quán dữ liệu
- Làm cơ sở cho việc kiểm thử
- Giúp tránh sai lệch giữa thiết kế và implementation

Các quy tắc này được rút ra trực tiếp từ:
- thiết kế database
- logic API
- hành vi Web Admin
- và cách hệ thống được triển khai thực tế.

---

## 8. Thiết kế dữ liệu (Data Design)

Phần này mô tả cấu trúc dữ liệu của hệ thống, bao gồm các thực thể chính, mối quan hệ giữa chúng và các ràng buộc dữ liệu. Thiết kế dữ liệu được xây dựng dựa trên Entity Framework Core và lưu trữ bằng SQLite.

---

### 8.1. Tổng quan mô hình dữ liệu

Hệ thống sử dụng mô hình dữ liệu quan hệ với các nhóm thực thể chính:

| Nhóm | Thực thể |
|---|---|
| Địa điểm | `FoodStall`, `FoodStallTranslation` |
| Ngôn ngữ | `Language` |
| Tour | `Tour`, `TourItem`, `TourTranslation` |
| Playback | `PlaybackLog` |
| Quản trị | `ApplicationUser`, `IdentityRole` |

Mô hình được thiết kế theo hướng:
- tách dữ liệu nội dung và dữ liệu đa ngôn ngữ
- đảm bảo khả năng mở rộng thêm ngôn ngữ
- hỗ trợ quản lý quan hệ giữa các thực thể

---

### 8.2. Sơ đồ quan hệ dữ liệu

**[TODO - Chèn ERD (Entity Relationship Diagram)]**

Gợi ý:
- FoodStall — FoodStallTranslation (1-N)
- Language — FoodStallTranslation (1-N)
- Tour — TourItem (1-N)
- Tour — TourTranslation (1-N)
- FoodStall — TourItem (1-N)
- FoodStall — PlaybackLog (1-N)

**[TODO - Chèn UML Class Diagram]**

---

### 8.3. Thực thể FoodStall

#### 8.3.1. Mô tả

`FoodStall` đại diện cho một địa điểm (quán ăn) trong hệ thống.

#### 8.3.2. Thuộc tính

| Thuộc tính | Kiểu | Mô tả |
|---|---|---|
| Id | int | Khóa chính |
| Latitude | double | Vĩ độ |
| Longitude | double | Kinh độ |
| Radius | double | Bán kính kích hoạt (mét) |
| Priority | int | Độ ưu tiên |
| ImageUrl | string | URL hình ảnh |
| Address | string | Địa chỉ |
| PriceRange | string | Khoảng giá |
| MapLink | string | Link bản đồ |
| IsActive | bool | Trạng thái hoạt động |

#### 8.3.3. Quan hệ

| Quan hệ | Loại |
|---|---|
| FoodStall → FoodStallTranslation | 1 - N |
| FoodStall → PlaybackLog | 1 - N |
| FoodStall → TourItem | 1 - N |

---

### 8.4. Thực thể FoodStallTranslation

#### 8.4.1. Mô tả

Lưu nội dung mô tả của địa điểm theo từng ngôn ngữ.

#### 8.4.2. Thuộc tính

| Thuộc tính | Kiểu | Mô tả |
|---|---|---|
| Id | int | Khóa chính |
| FoodStallId | int | FK tới FoodStall |
| LanguageId | int | FK tới Language |
| Name | string | Tên quán |
| Description | string | Mô tả |
| Specialty | string | Món đặc trưng |
| AudioUrl | string | URL audio |

#### 8.4.3. Ràng buộc

| Ràng buộc | Mô tả |
|---|---|
| Unique (FoodStallId, LanguageId) | Mỗi quán chỉ có 1 translation mỗi ngôn ngữ |

---

### 8.5. Thực thể Language

#### 8.5.1. Mô tả

Lưu danh sách các ngôn ngữ được hỗ trợ trong hệ thống.

#### 8.5.2. Thuộc tính

| Thuộc tính | Kiểu | Mô tả |
|---|---|---|
| Id | int | Khóa chính |
| LanguageCode | string | Mã ngôn ngữ (vi, en) |
| DisplayName | string | Tên hiển thị |
| FlagIcon | string | Icon cờ |
| IntroText | string | Nội dung giới thiệu |
| IsDefault | bool | Ngôn ngữ mặc định |

#### 8.5.3. Ràng buộc

| Ràng buộc | Mô tả |
|---|---|
| LanguageCode unique | Không trùng mã ngôn ngữ |

---

### 8.6. Thực thể PlaybackLog

#### 8.6.1. Mô tả

Lưu lịch sử phát audio của người dùng.

#### 8.6.2. Thuộc tính

| Thuộc tính | Kiểu | Mô tả |
|---|---|---|
| Id | int | Khóa chính |
| FoodStallId | int | FK tới FoodStall |
| LanguageCode | string | Ngôn ngữ phát |
| TriggerType | string | Loại trigger (GPS) |
| PlayedAt | datetime | Thời điểm phát |
| DurationSeconds | int | Thời lượng phát |

#### 8.6.3. Ghi chú

- `PlayedAt` được ghi bằng thời gian server (UTC)
- `DurationSeconds` được gửi từ mobile app

---

### 8.7. Thực thể Tour

#### 8.7.1. Mô tả

Đại diện cho một tour gồm nhiều điểm tham quan.

#### 8.7.2. Thuộc tính

| Thuộc tính | Kiểu | Mô tả |
|---|---|---|
| Id | int | Khóa chính |
| IsActive | bool | Trạng thái |

#### 8.7.3. Quan hệ

| Quan hệ | Loại |
|---|---|
| Tour → TourItem | 1 - N |
| Tour → TourTranslation | 1 - N |

---

### 8.8. Thực thể TourItem

#### 8.8.1. Mô tả

Liên kết giữa Tour và FoodStall, xác định các điểm trong tour.

#### 8.8.2. Thuộc tính

| Thuộc tính | Kiểu | Mô tả |
|---|---|---|
| Id | int | Khóa chính |
| TourId | int | FK tới Tour |
| FoodStallId | int | FK tới FoodStall |
| OrderIndex | int | Thứ tự trong tour |

#### 8.8.3. Ràng buộc

| Ràng buộc | Mô tả |
|---|---|
| Unique (TourId, FoodStallId) | Không lặp lại quán trong tour |

---

### 8.9. Thực thể TourTranslation

#### 8.9.1. Mô tả

Lưu nội dung tour theo từng ngôn ngữ.

#### 8.9.2. Thuộc tính

| Thuộc tính | Kiểu | Mô tả |
|---|---|---|
| Id | int | Khóa chính |
| TourId | int | FK tới Tour |
| LanguageId | int | FK tới Language |
| Name | string | Tên tour |
| Description | string | Mô tả |

#### 8.9.3. Ràng buộc

| Ràng buộc | Mô tả |
|---|---|
| Unique (TourId, LanguageId) | 1 translation mỗi ngôn ngữ |

---

### 8.10. Thực thể ApplicationUser

#### 8.10.1. Mô tả

Đại diện cho người dùng quản trị hệ thống.

#### 8.10.2. Thuộc tính chính

| Thuộc tính | Mô tả |
|---|---|
| Id | ID user |
| Email | Email đăng nhập |
| FullName | Tên đầy đủ |
| PasswordHash | Mật khẩu mã hóa |

#### 8.10.3. Quan hệ

- Thuộc hệ thống Identity
- Gắn với `IdentityRole`

---

### 8.11. Mối quan hệ tổng thể

| Thực thể A | Thực thể B | Quan hệ |
|---|---|---|
| FoodStall | FoodStallTranslation | 1 - N |
| Language | FoodStallTranslation | 1 - N |
| Tour | TourItem | 1 - N |
| Tour | TourTranslation | 1 - N |
| FoodStall | TourItem | 1 - N |
| FoodStall | PlaybackLog | 1 - N |

---

### 8.12. Đặc điểm thiết kế dữ liệu

| Đặc điểm | Mô tả |
|---|---|
| Hỗ trợ đa ngôn ngữ | Tách bảng translation |
| Dữ liệu chuẩn hóa | Không lặp nội dung giữa các bảng |
| Quan hệ rõ ràng | FK + unique constraint |
| Dễ mở rộng | Có thể thêm ngôn ngữ mới |
| Tách auth và domain | DB riêng cho Identity |

---

### 8.13. Tổng kết

Thiết kế dữ liệu của hệ thống:
- phản ánh đầy đủ domain nghiệp vụ
- hỗ trợ tốt cho đa ngôn ngữ
- đảm bảo tính nhất quán và toàn vẹn dữ liệu
- phù hợp với kiến trúc hiện tại của hệ thống

Mô hình này là nền tảng cho:
- Backend API
- Mobile App
- Web Admin

---
## 9. Thiết kế API (API Design Overview)

Phần này mô tả các API chính của hệ thống Backend, bao gồm các endpoint cung cấp dữ liệu cho mobile app và endpoint ghi nhận dữ liệu từ phía client.

API được thiết kế theo mô hình RESTful, sử dụng HTTP và JSON làm định dạng trao đổi dữ liệu.

---

### 9.1. Tổng quan

| Thuộc tính | Giá trị |
|---|---|
| Giao thức | HTTP |
| Kiểu API | RESTful |
| Định dạng dữ liệu | JSON |
| Base URL | `/api/...` |
| Xác thực | Không yêu cầu (đối với mobile API hiện tại) |

---

### 9.2. Nhóm API FoodStall

#### 9.2.1. Lấy danh sách địa điểm

- **Endpoint**: `GET /api/FoodStalls`
- **Mô tả**: Trả danh sách các quán ăn đang hoạt động

##### Query Parameters

| Tên | Kiểu | Bắt buộc | Mô tả |
|---|---|---|---|
| lang | string | Không | Mã ngôn ngữ (mặc định: `vi`) |

##### Response

~~~json
[
  {
    "id": 1,
    "name": "Bánh mì A",
    "address": "...",
    "specialty": "...",
    "priceRange": "...",
    "imageUrl": "...",
    "latitude": 10.123,
    "longitude": 106.123,
    "radius": 35,
    "description": "...",
    "audioUrl": "...",
    "priority": 0,
    "mapLink": "...",
    "languageCode": "vi"
  }
]
~~~

##### Ghi chú

- Chỉ trả các `FoodStall` có `IsActive = true`
- Hỗ trợ đa ngôn ngữ với fallback về default language
- Sắp xếp theo `Priority`

---

#### 9.2.2. Lấy chi tiết địa điểm

- **Endpoint**: `GET /api/FoodStalls/{id}`
- **Mô tả**: Trả thông tin chi tiết của một địa điểm

##### Query Parameters

| Tên | Kiểu | Bắt buộc | Mô tả |
|---|---|---|---|
| lang | string | Không | Mã ngôn ngữ |

##### Response

~~~json
{
  "id": 1,
  "name": "...",
  "address": "...",
  "specialty": "...",
  "priceRange": "...",
  "imageUrl": "...",
  "latitude": 10.123,
  "longitude": 106.123,
  "radius": 35,
  "description": "...",
  "audioUrl": "...",
  "priority": 0,
  "mapLink": "...",
  "languageCode": "vi"
}
~~~

---

### 9.3. Nhóm API Language

#### 9.3.1. Lấy danh sách ngôn ngữ

- **Endpoint**: `GET /api/Languages`
- **Mô tả**: Trả danh sách các ngôn ngữ được hỗ trợ

##### Response

~~~json
[
  {
    "languageCode": "vi",
    "displayName": "Tiếng Việt",
    "flagIcon": "/flags/vn.png",
    "introText": "...",
    "isDefault": true
  }
]
~~~

##### Ghi chú

- Ngôn ngữ mặc định được xác định bởi `IsDefault = true`

---

### 9.4. Nhóm API PlaybackLog

#### 9.4.1. Gửi playback log

- **Endpoint**: `POST /api/PlaybackLogs`
- **Mô tả**: Ghi nhận một lần phát audio từ mobile app

##### Request Body

~~~json
{
  "foodStallId": 1,
  "languageCode": "vi",
  "triggerType": "GPS",
  "durationSeconds": 35
}
~~~

##### Response

~~~json
{
  "message": "Lưu lịch sử nghe audio thành công!"
}
~~~

##### Ghi chú

- `PlayedAt` được backend tự gán bằng thời gian UTC
- `DurationSeconds` được gửi từ mobile app
- API không làm gián đoạn trải nghiệm nếu log thất bại phía client

---

### 9.5. Nhóm API Tour

#### 9.5.1. Lấy danh sách tour

- **Endpoint**: `GET /api/Tours`
- **Mô tả**: Trả danh sách các tour đang hoạt động

##### Response (rút gọn)

~~~json
[
  {
    "id": 1,
    "isActive": true,
    "translations": [
      {
        "name": "Tour ẩm thực",
        "description": "...",
        "language": {
          "languageCode": "vi"
        }
      }
    ],
    "tourItems": [
      {
        "foodStallId": 1,
        "orderIndex": 1
      }
    ]
  }
]
~~~

##### Ghi chú

- Trả dữ liệu gồm: tour, translation và danh sách điểm
- Chỉ trả các tour có `IsActive = true`
- Hiện chưa sử dụng DTO riêng cho mobile

---

### 9.6. Quy tắc xử lý API

| Quy tắc | Mô tả |
|---|---|
| RESTful design | Sử dụng HTTP method chuẩn (GET, POST) |
| JSON serialization | Dữ liệu trả về ở dạng JSON |
| Ignore cycles | Tránh vòng lặp khi serialize entity |
| AsNoTracking | Tối ưu truy vấn đọc |
| Error handling | Trả mã lỗi phù hợp (400, 404, 500) |
| DTO mapping | Dùng DTO cho mobile (FoodStall) |

---

### 9.7. Xử lý đa ngôn ngữ trong API

| Bước | Mô tả |
|---|---|
| 1 | Nhận `lang` từ query |
| 2 | Tìm language tương ứng |
| 3 | Nếu không có → dùng default language |
| 4 | Lấy translation theo language |
| 5 | Nếu thiếu → fallback |
| 6 | Trả dữ liệu về client |

---

### 9.8. Luồng dữ liệu tổng quan

| Luồng | Mô tả |
|---|---|
| App → API | Lấy danh sách quán, ngôn ngữ, tour |
| API → DB | Truy vấn dữ liệu |
| App → API | Gửi playback log |
| API → DB | Lưu log |
| Admin → DB | Quản lý dữ liệu trực tiếp |

---

### 9.9. Giới hạn hiện tại của API

| Vấn đề | Mô tả |
|---|---|
| Chưa có authentication | API mở |
| Tour chưa có DTO | Trả entity trực tiếp |
| Không có pagination | Trả toàn bộ dữ liệu |
| Không có caching | Query trực tiếp DB |
| Không có versioning | Chưa hỗ trợ version |

---

### 9.10. Tổng kết

API hiện tại:
- đáp ứng đầy đủ nhu cầu mobile app
- hỗ trợ đa ngôn ngữ
- có thiết kế đơn giản, dễ sử dụng
- phù hợp với quy mô đồ án

Hướng cải thiện trong tương lai:
- chuẩn hóa DTO cho tour
- bổ sung authentication
- tối ưu performance khi dữ liệu tăng

---
## 10. Thiết kế giao diện (UI/UX Overview)

Phần này mô tả tổng quan các màn hình chính của hệ thống, bao gồm Mobile App và Web Admin. Mục tiêu là xác định các thành phần giao diện, chức năng chính của từng màn hình và cách người dùng tương tác với hệ thống.

---

### 10.1. Nguyên tắc thiết kế

| Nguyên tắc | Mô tả |
|---|---|
| Đơn giản | Giao diện dễ sử dụng, giảm thao tác |
| Trực quan | Thông tin rõ ràng, dễ hiểu |
| Phản hồi nhanh | Hệ thống phản hồi kịp thời khi người dùng thao tác |
| Phù hợp mobile | Ưu tiên trải nghiệm trên thiết bị di động |
| Nhất quán | Các màn hình có thiết kế đồng nhất |

---

## 10.2. Mobile App

Mobile App là thành phần trực tiếp tương tác với người dùng cuối, tập trung vào trải nghiệm khám phá và nghe audio.

---

### 10.2.1. Màn hình danh sách địa điểm

#### Mô tả

Hiển thị danh sách các quán ăn (POI) để người dùng lựa chọn hoặc tham khảo.

#### Thành phần chính

| Thành phần | Mô tả |
|---|---|
| Danh sách POI | Hiển thị tên, địa chỉ, hình ảnh |
| Thanh tìm kiếm (nếu có) | Lọc địa điểm |
| Item POI | Có thể click để xem chi tiết |

#### Chức năng

| Chức năng | Mô tả |
|---|---|
| Xem danh sách | Hiển thị tất cả quán |
| Xem chi tiết | Click vào một quán |
| Chuyển sang bản đồ | Điều hướng sang map |

**[TODO - Chèn wireframe màn hình danh sách POI]**

---

### 10.2.2. Màn hình bản đồ (Map Screen)

#### Mô tả

Hiển thị vị trí người dùng và các POI trên bản đồ.

#### Thành phần chính

| Thành phần | Mô tả |
|---|---|
| Bản đồ | Hiển thị Google Map hoặc tương đương |
| Marker POI | Đánh dấu vị trí các quán |
| Marker người dùng | Hiển thị vị trí hiện tại |

#### Chức năng

| Chức năng | Mô tả |
|---|---|
| Theo dõi vị trí | Cập nhật GPS realtime |
| Hiển thị POI | Vẽ marker theo tọa độ |
| Trigger audio | Khi vào vùng bán kính |

**[TODO - Chèn wireframe màn hình bản đồ]**

---

### 10.2.3. Màn hình chi tiết địa điểm

#### Mô tả

Hiển thị thông tin chi tiết của một quán ăn.

#### Thành phần chính

| Thành phần | Mô tả |
|---|---|
| Tên quán | Theo ngôn ngữ |
| Hình ảnh | Từ ImageUrl |
| Mô tả | Description |
| Specialty | Món đặc trưng |
| Giá | PriceRange |
| Nút phát audio | Phát thủ công |

#### Chức năng

| Chức năng | Mô tả |
|---|---|
| Xem thông tin | Hiển thị đầy đủ |
| Phát audio | Thủ công nếu cần |
| Mở bản đồ | Qua MapLink |

**[TODO - Chèn wireframe màn hình chi tiết]**

---

### 10.2.4. Màn hình chọn ngôn ngữ

#### Mô tả

Cho phép người dùng chọn ngôn ngữ hiển thị.

#### Thành phần

| Thành phần | Mô tả |
|---|---|
| Danh sách ngôn ngữ | Hiển thị DisplayName + FlagIcon |
| Ngôn ngữ mặc định | Được đánh dấu |

#### Chức năng

| Chức năng | Mô tả |
|---|---|
| Chọn ngôn ngữ | Áp dụng toàn app |
| Lưu lựa chọn | Dùng cho API |

**[TODO - Chèn wireframe màn hình chọn ngôn ngữ]**

---

### 10.2.5. Màn hình Tour

#### Mô tả

Hiển thị danh sách tour và cho phép người dùng tham gia.

#### Thành phần

| Thành phần | Mô tả |
|---|---|
| Danh sách tour | Tên + mô tả |
| Danh sách điểm | Các POI trong tour |
| Trạng thái | Đang tham gia / chưa |

#### Chức năng

| Chức năng | Mô tả |
|---|---|
| Xem tour | Danh sách tour |
| Chọn tour | Bắt đầu |
| Theo dõi tiến trình | Theo thứ tự |

**[TODO - Chèn wireframe màn hình tour]**

---

## 10.3. Web Admin

Web Admin là công cụ quản trị dữ liệu và giám sát hệ thống.

---

### 10.3.1. Màn hình đăng nhập

#### Thành phần

| Thành phần | Mô tả |
|---|---|
| Email | Input |
| Password | Input |
| Nút login | Submit |

#### Chức năng

| Chức năng | Mô tả |
|---|---|
| Đăng nhập | Xác thực user |
| Redirect | Vào dashboard |

**[TODO - Chèn wireframe login]**

---

### 10.3.2. Dashboard

#### Mô tả

Hiển thị tổng quan hệ thống.

#### Thành phần

| Thành phần | Mô tả |
|---|---|
| Tổng số quán | Count |
| Tổng log | Count |
| Thống kê GPS | Số lượt |
| Thống kê ngôn ngữ | vi/en |
| Recent logs | 5 bản ghi |
| Top quán | Theo lượt phát |

**[TODO - Chèn wireframe dashboard]**

---

### 10.3.3. Quản lý FoodStall

#### Thành phần

| Thành phần | Mô tả |
|---|---|
| Danh sách | Table |
| Search | Theo tên/địa chỉ |
| Filter | Active/inactive |

#### Chức năng

| Chức năng | Mô tả |
|---|---|
| Create | Tạo mới |
| Edit | Sửa |
| Delete | Xóa |
| Edit Translation | Quản lý ngôn ngữ |

**[TODO - Chèn wireframe FoodStall CRUD]**

---

### 10.3.4. Quản lý Tour

#### Thành phần

| Thành phần | Mô tả |
|---|---|
| Danh sách tour | Table |
| Chi tiết tour | Danh sách điểm |

#### Chức năng

| Chức năng | Mô tả |
|---|---|
| Create | Tạo tour |
| Edit | Sửa |
| Delete | Xóa |
| Sắp thứ tự | OrderIndex |

**[TODO - Chèn wireframe Tour]**

---

### 10.3.5. Playback Logs

#### Thành phần

| Thành phần | Mô tả |
|---|---|
| Danh sách log | Table |
| Filter | Language, trigger, POI |

#### Chức năng

| Chức năng | Mô tả |
|---|---|
| Xem log | Lịch sử |
| Lọc | Theo điều kiện |
| Xem chi tiết | Duration, thời gian |

**[TODO - Chèn wireframe logs]**

---

### 10.3.6. Quản lý User

#### Thành phần

| Thành phần | Mô tả |
|---|---|
| Danh sách user | Table |
| Role | Admin / FoodStallOwner |

#### Chức năng

| Chức năng | Mô tả |
|---|---|
| Create user | Tạo tài khoản |
| Assign role | Gán quyền |

**[TODO - Chèn wireframe user management]**

---

### 10.4. Tổng kết

Thiết kế UI/UX của hệ thống:
- tập trung vào trải nghiệm đơn giản và trực quan
- ưu tiên mobile-first cho người dùng cuối
- cung cấp đầy đủ công cụ quản trị cho admin
- hỗ trợ mở rộng thêm tính năng trong tương lai

---
## 11. Yêu cầu phi chức năng (Non-functional Requirements)

Phần này mô tả các yêu cầu phi chức năng của hệ thống, bao gồm các tiêu chí về hiệu năng, độ tin cậy, khả năng mở rộng, bảo mật và khả năng bảo trì. Các yêu cầu này đảm bảo hệ thống hoạt động ổn định và phù hợp với môi trường triển khai thực tế.

---

### 11.1. Hiệu năng (Performance)

| Mã | Yêu cầu | Mô tả |
|---|---|---|
| NFR-P1 | Thời gian phản hồi API | Các API chính nên phản hồi dưới 1 giây trong điều kiện bình thường |
| NFR-P2 | Tải dữ liệu ban đầu | Mobile app phải tải danh sách POI trong thời gian chấp nhận được |
| NFR-P3 | Xử lý GPS realtime | App phải xử lý vị trí người dùng gần realtime |
| NFR-P4 | Phát audio không delay | Audio phải được kích hoạt nhanh khi vào vùng trigger |
| NFR-P5 | Truy vấn DB hiệu quả | Sử dụng `AsNoTracking` cho truy vấn read-only |

---

### 11.2. Độ tin cậy (Reliability)

| Mã | Yêu cầu | Mô tả |
|---|---|---|
| NFR-R1 | Không crash khi lỗi API | Mobile app vẫn hoạt động nếu API lỗi |
| NFR-R2 | Playback không bị gián đoạn | Việc gửi log không ảnh hưởng audio |
| NFR-R3 | Dữ liệu nhất quán | Domain DB dùng chung giữa API và Admin |
| NFR-R4 | Migration tự động | DB luôn được cập nhật schema khi chạy |
| NFR-R5 | Fallback ngôn ngữ | Luôn có dữ liệu hiển thị |

---

### 11.3. Khả năng mở rộng (Scalability)

| Mã | Yêu cầu | Mô tả |
|---|---|---|
| NFR-S1 | Mở rộng dữ liệu | Có thể thêm nhiều FoodStall |
| NFR-S2 | Mở rộng ngôn ngữ | Có thể thêm Language mới |
| NFR-S3 | Mở rộng tour | Có thể thêm nhiều Tour |
| NFR-S4 | Tách layer rõ ràng | App, API, Admin độc lập |
| NFR-S5 | Có thể nâng cấp DB | Có thể chuyển từ SQLite sang DB khác |

---

### 11.4. Khả năng bảo trì (Maintainability)

| Mã | Yêu cầu | Mô tả |
|---|---|---|
| NFR-M1 | Code dễ đọc | Sử dụng cấu trúc rõ ràng |
| NFR-M2 | Tách DTO | Không expose trực tiếp entity |
| NFR-M3 | Tách domain và auth | 2 DB riêng biệt |
| NFR-M4 | Sử dụng EF Core | Dễ maintain và migrate |
| NFR-M5 | Có seed data | Dễ khởi tạo hệ thống |

---

### 11.5. Bảo mật (Security)

| Mã | Yêu cầu | Mô tả |
|---|---|---|
| NFR-SE1 | Xác thực admin | Sử dụng ASP.NET Identity |
| NFR-SE2 | Phân quyền role | Admin và FoodStallOwner |
| NFR-SE3 | Bảo vệ endpoint | Dùng `[Authorize]` |
| NFR-SE4 | Password policy | Tối thiểu 6 ký tự |
| NFR-SE5 | Không public admin API | Web Admin không public |

---

### 11.6. Khả năng sử dụng (Usability)

| Mã | Yêu cầu | Mô tả |
|---|---|---|
| NFR-U1 | Giao diện đơn giản | Dễ sử dụng cho người dùng |
| NFR-U2 | Trải nghiệm tự động | GPS trigger giảm thao tác |
| NFR-U3 | Hỗ trợ đa ngôn ngữ | Người dùng chọn language |
| NFR-U4 | Phản hồi rõ ràng | UI hiển thị trạng thái |
| NFR-U5 | Dễ quản trị | Admin dễ thao tác |

---

### 11.7. Khả năng tương thích (Compatibility)

| Mã | Yêu cầu | Mô tả |
|---|---|---|
| NFR-C1 | Android support | App chạy trên Android |
| NFR-C2 | API tương thích mobile | Không thay đổi breaking |
| NFR-C3 | Web Admin chạy browser | Không cần cài đặt |
| NFR-C4 | JSON standard | Dễ tích hợp |
| NFR-C5 | Static files | Hỗ trợ ảnh/audio |

---

### 11.8. Logging và Monitoring

| Mã | Yêu cầu | Mô tả |
|---|---|---|
| NFR-L1 | Ghi nhận playback | Lưu vào PlaybackLog |
| NFR-L2 | Theo dõi usage | Dùng dashboard |
| NFR-L3 | Recent logs | Hiển thị log gần |
| NFR-L4 | Top usage | Thống kê quán |
| NFR-L5 | Filter log | Theo language, trigger |

---

### 11.9. Giới hạn hệ thống

| Mã | Giới hạn | Mô tả |
|---|---|---|
| NFR-LIM1 | SQLite | Không phù hợp hệ thống lớn |
| NFR-LIM2 | Không caching | Chưa tối ưu hiệu năng |
| NFR-LIM3 | Không auth API | API mở |
| NFR-LIM4 | Không pagination | Trả toàn bộ dữ liệu |
| NFR-LIM5 | Không realtime sync | Không đồng bộ nhiều thiết bị |

---

### 11.10. Tổng kết

Các yêu cầu phi chức năng đảm bảo rằng hệ thống:
- hoạt động ổn định
- dễ mở rộng
- dễ bảo trì
- phù hợp với quy mô đồ án

Đồng thời cung cấp nền tảng để nâng cấp hệ thống trong tương lai khi quy mô dữ liệu và số lượng người dùng tăng lên.

---
## 12. Kết luận hệ thống

Tài liệu PRD này mô tả toàn diện hệ thống Audio Guide Phố Ẩm Thực Vĩnh Khánh, bao gồm mục tiêu sản phẩm, kiến trúc hệ thống, luồng nghiệp vụ, yêu cầu chức năng, thiết kế dữ liệu và thiết kế API.

---

### 12.1. Tổng kết hệ thống

Hệ thống được xây dựng theo mô hình client-server với các thành phần chính:

| Thành phần | Vai trò |
|---|---|
| Mobile App | Tương tác với người dùng, xử lý GPS và phát audio |
| Backend API | Cung cấp dữ liệu và ghi nhận hành vi |
| Database | Lưu trữ dữ liệu nghiệp vụ |
| Web Admin | Quản trị nội dung và giám sát hệ thống |

Giải pháp tập trung vào việc sử dụng GPS để kích hoạt nội dung audio theo vị trí, kết hợp với hỗ trợ đa ngôn ngữ và hệ thống quản trị dữ liệu.

---

### 12.2. Điểm mạnh của hệ thống

| Nhóm | Mô tả |
|---|---|
| Kiến trúc rõ ràng | Phân tách mobile, API, admin và database |
| Đa ngôn ngữ | Thiết kế translation mở rộng tốt |
| Trải nghiệm tự động | GPS trigger giảm thao tác người dùng |
| Quản trị đầy đủ | Web Admin hỗ trợ CRUD và dashboard |
| Dữ liệu nhất quán | API và Admin dùng chung domain database |
| Khả năng mở rộng | Có thể thêm POI, tour và ngôn ngữ |

---

### 12.3. Hạn chế hiện tại

| Nhóm | Mô tả |
|---|---|
| Hiệu năng | Chưa có caching hoặc tối ưu nâng cao |
| API | Chưa có authentication và versioning |
| Dữ liệu | Sử dụng SQLite nên giới hạn quy mô |
| Tour API | Chưa sử dụng DTO riêng |
| Mobile | Một số logic quan trọng nằm hoàn toàn phía client |
| Logging | Chưa có hệ thống monitoring chuyên sâu |

---

### 12.4. Định hướng phát triển

Hệ thống có thể được mở rộng trong các giai đoạn tiếp theo:

| Hướng phát triển | Mô tả |
|---|---|
| Tối ưu API | Thêm pagination, caching, DTO chuẩn hóa |
| Bảo mật | Thêm authentication cho API |
| Mở rộng dữ liệu | Hỗ trợ nhiều ngôn ngữ hơn |
| Nâng cấp database | Chuyển sang SQL Server hoặc PostgreSQL |
| Analytics nâng cao | Phân tích hành vi người dùng |
| UI/UX cải tiến | Tối ưu trải nghiệm mobile |

---

### 12.5. Kết luận

Hệ thống Audio Guide Phố Ẩm Thực Vĩnh Khánh đáp ứng tốt mục tiêu xây dựng một giải pháp hỗ trợ khám phá địa điểm ẩm thực bằng công nghệ GPS và audio.

Thiết kế hiện tại:
- phù hợp với quy mô đồ án
- có tính thực tiễn cao
- dễ triển khai và mở rộng

Tài liệu PRD này đóng vai trò làm cơ sở cho:
- phát triển hệ thống
- kiểm thử
- đánh giá và bảo vệ đồ án

Đồng thời cung cấp nền tảng để tiếp tục nâng cấp hệ thống trong tương lai.