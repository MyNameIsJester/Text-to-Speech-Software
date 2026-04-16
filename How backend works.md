# System Features and App Integration Guide
## Audio Guide Phố Ẩm Thực Vĩnh Khánh

Tài liệu này mô tả ngắn gọn các feature hiện có của hệ thống backend và web admin, đồng thời giải thích sơ lược app mobile cần tích hợp như thế nào để làm việc đúng với hệ thống.

---

# 1. Tổng quan kiến trúc

Hệ thống hiện tại gồm 3 phần chính:

- **Mobile App (.NET MAUI)**  
  Dùng để hiển thị quán, phát nội dung audio/TTS, trigger bằng GPS hoặc QR, và ghi nhận lịch sử phát.

- **Backend API (ASP.NET Core Web API + SQLite)**  
  Là trung tâm xử lý dữ liệu. App và Web Admin đều đọc/ghi dữ liệu thông qua backend.

- **Web Admin (ASP.NET Core MVC)**  
  Dùng để quản lý dữ liệu nội dung và vận hành hệ thống.

## Quan hệ giữa các phần

- Web Admin **không giao tiếp trực tiếp với app**
- App **không giao tiếp trực tiếp với Web Admin**
- Cả hai cùng làm việc với:
  - **Backend API**
  - **Database SQLite**

---

# 2. Các feature chính của Backend

## 2.1 Language
Bảng ngôn ngữ dùng để quản lý các ngôn ngữ hệ thống.

Hiện đang có:
- `vi`
- `en`

### Mục đích
- xác định ngôn ngữ mặc định
- dùng để gắn translation cho quán và tour

---

## 2.2 FoodStall
Đây là thực thể chính của hệ thống, đại diện cho một quán/điểm dừng.

### Dữ liệu chính
- `Id`
- `Latitude`
- `Longitude`
- `Radius`
- `ImageUrl`
- `Address`
- `PriceRange`
- `Priority`
- `MapLink`
- `IsActive`

### Mục đích
- dùng cho GPS trigger
- dùng cho QR resolve
- dùng để hiển thị thông tin quán trong app
- dùng trong tour

---

## 2.3 FoodStallTranslation
Đây là nội dung đa ngôn ngữ cho từng quán.

### Mỗi translation gồm
- `Name`
- `Description`
- `Specialty`
- `AudioUrl` (optional)

### Lưu ý quan trọng
Hiện tại hệ thống đã đơn giản hóa:
- **TTS đọc trực tiếp từ `Description`**
- không còn dùng `TtsScript` riêng nữa

### Mục đích
- app chọn đúng translation theo ngôn ngữ hiện tại
- UI hiển thị `Name`, `Description`, `Specialty`
- TTS đọc `Description`

---

## 2.4 PlaybackLog
Đây là log runtime, ghi lại mỗi lần app phát nội dung.

### Dữ liệu chính
- `FoodStallId`
- `LanguageCode`
- `TriggerType`
- `PlayedAt`
- `DurationSeconds`

### TriggerType hiện có
- `GPS`
- `QR`

### Mục đích
- theo dõi lịch sử phát nội dung
- phục vụ thống kê và debug
- dùng trên Web Admin Dashboard và Playback Logs

---

## 2.5 QrMapping
Đây là mapping giữa mã QR và quán.

### Dữ liệu chính
- `FoodStallId`
- `CodeValue`
- `IsActive`

### Giải thích
`CodeValue` **không phải hình QR**, mà là **text được chứa trong QR**.

Ví dụ:
- QR image chứa text: `STALL-001`
- app scan ra: `STALL-001`
- backend dùng `CodeValue = STALL-001` để tìm quán tương ứng

### Mục đích
- app scan QR
- app gọi API resolve QR
- backend trả về quán tương ứng

---

## 2.6 Tour
Đây là một hành trình gồm nhiều quán theo thứ tự.

### Tour hiện tại gồm 3 phần

#### a. Tour
- `Id`
- `IsActive`

#### b. TourTranslation
- `LanguageId`
- `Name`
- `Description`

#### c. TourItem
- `TourId`
- `FoodStallId`
- `OrderIndex`

### Mục đích
- tạo tour đa ngôn ngữ
- sắp quán theo thứ tự trải nghiệm

### Lưu ý quan trọng
`OrderIndex` là:
- thứ tự đi
- thứ tự phát
- thứ tự stop trong tour

Nó **không phải id**, mà là logic thứ tự của hành trình.

---

# 3. Các feature chính của Web Admin

Web Admin hiện đã gần hoàn chỉnh cho v1.

## 3.1 Food Stalls
Cho phép:
- xem danh sách quán
- tạo quán mới
- sửa thông tin base
- xóa quán
- search theo tên/địa chỉ
- filter theo active/inactive

---

## 3.2 Food Stall Translations
Cho phép:
- sửa nội dung `vi`
- sửa nội dung `en`

### Các field chính
- Name
- Description
- Specialty
- AudioUrl

### Mục đích
- quản lý nội dung app sẽ hiển thị và đọc

---

## 3.3 Playback Logs
Cho phép:
- xem log phát nội dung
- filter theo:
  - language
  - trigger type
  - food stall
- xem summary đơn giản:
  - total logs
  - GPS logs
  - QR logs
  - VI logs
  - EN logs

### Mục đích
- hỗ trợ debug app
- kiểm tra runtime
- dùng để demo hệ thống hoạt động

---

## 3.4 QR Mappings
Cho phép:
- tạo QR mapping
- sửa QR mapping
- xóa QR mapping
- bật/tắt QR mapping

### Mục đích
- quản lý mã QR đang trỏ tới quán nào
- đổi mapping mà không cần sửa app

---

## 3.5 Tours
Cho phép:
- tạo tour
- sửa tour
- xóa tour
- nhập nội dung đa ngôn ngữ cho tour (`vi`, `en`)
- thêm stop động
- xóa stop động
- chọn quán cho từng stop bằng searchable input
- bắt buộc tour phải có ít nhất 1 stop

### Mục đích
- tạo hành trình nhiều quán có thứ tự rõ ràng

---

## 3.6 Dashboard
Cho phép xem tổng quan hệ thống:
- total food stalls
- active food stalls
- total playback logs
- total QR mappings
- total tours
- GPS/QR logs
- VI/EN logs
- recent playback logs
- top food stalls by playback

### Mục đích
- theo dõi tổng quan
- phục vụ demo và báo cáo

---

# 4. App cần tích hợp như thế nào

App mobile cần làm việc với backend theo đúng các luồng sau.

---

# 5. Tích hợp phần FoodStall + Translation

## App cần làm gì
- gọi API lấy danh sách FoodStalls
- lấy translation đúng theo ngôn ngữ hiện tại
- fallback về `vi` nếu thiếu translation

## App nên hiểu
`FoodStall` không còn chứa trực tiếp:
- Name
- Description

Nội dung nằm trong:
- `FoodStallTranslation`

## TTS
App nên:
- hiển thị `Description`
- đọc luôn `Description`

---

# 6. Tích hợp phần GPS

## Luồng mong muốn
1. app lấy vị trí hiện tại
2. tính khoảng cách tới các FoodStalls active
3. nếu khoảng cách <= `Radius` thì quán có thể được trigger
4. app chọn quán hợp lý
5. app hiển thị nội dung
6. app đọc `Description`
7. app ghi `PlaybackLog` với:
   - `TriggerType = GPS`

## Những gì app cần có
- location service
- distance calculator
- cooldown chống đọc lặp
- logic dùng `Priority` nếu nhiều quán cùng gần

---

# 7. Tích hợp phần QR

Đây là một trong những phần quan trọng nhất cần hiểu đúng.

## 7.1 Ý nghĩa của QR trong hệ thống
Web Admin chỉ quản lý:
- `CodeValue`
- quán tương ứng

Backend không lưu ảnh QR.

### Ví dụ
- QR image chứa text: `STALL-001`
- app scan ra `STALL-001`
- app gọi API resolve

## 7.2 API QR hiện có
Backend có API resolve QR.

Ví dụ:
`/api/qr/resolve?code=STALL-001&lang=vi`

## 7.3 Luồng app cần làm
1. scan QR
2. lấy text trong QR
3. gọi API resolve
4. backend trả về thông tin quán
5. app hiển thị nội dung
6. app đọc `Description`
7. app ghi `PlaybackLog` với:
   - `TriggerType = QR`

## 7.4 Nếu app chưa có thì cần làm gì
Nếu app hiện chỉ:
- scan xong hiện text
- hoặc hardcode QR → quán

thì cần sửa thành:
- scan QR → lấy text
- text → gọi backend resolve
- backend là nguồn dữ liệu chính

---

# 8. Tích hợp phần PlaybackLog

Đây là phần giúp web admin và dashboard biết app đang hoạt động thế nào.

## 8.1 App cần làm gì
Sau mỗi lần phát nội dung, app nên gọi API để ghi log.

## 8.2 Dữ liệu cần gửi
- `FoodStallId`
- `LanguageCode`
- `TriggerType`
- `PlayedAt`
- `DurationSeconds`

## 8.3 Khi nào cần ghi log
- khi GPS trigger và app bắt đầu phát
- khi QR trigger và app bắt đầu phát

## 8.4 Nếu app chưa có thì cần làm gì
- thêm API service để gửi PlaybackLog
- chuẩn hóa 2 trigger:
  - GPS
  - QR

## 8.5 Tại sao phần này quan trọng
Vì Web Admin hiện đã có:
- Playback Logs page
- Dashboard summary

Nếu app không ghi log thì web admin sẽ thiếu dữ liệu runtime.

---

# 9. Tích hợp phần Tour

Đây là phần app rất dễ lệch với backend nếu đang dùng model cũ.

## 9.1 Tour hiện tại là đa ngôn ngữ
Không còn là:
- `Tour.Name`
- `Tour.Description`

Mà là:
- `Tour`
- `TourTranslation`
- `TourItem`

## 9.2 App cần làm gì
- load Tour
- chọn `TourTranslation` theo ngôn ngữ hiện tại
- load `TourItems`
- sort `TourItems` theo `OrderIndex`

## 9.3 Ý nghĩa của `OrderIndex`
Đây là thứ tự stop trong hành trình.

Ví dụ:
- stop 1 → quán A
- stop 2 → quán B
- stop 3 → quán C

App phải hiển thị và xử lý theo thứ tự này.

## 9.4 Nếu app chưa có thì cần làm gì
Nếu app đang dùng:
- `Tour.Name`
- `Tour.Description`

thì phải refactor sang:
- `TourTranslation`
- `TourItem`

Nếu app đang load stop nhưng chưa sort:
- cần sort theo `OrderIndex`

---

# 10. Những điểm app rất dễ bị lệch so với backend hiện tại

## 10.1 Dùng model cũ một ngôn ngữ
Sai nếu app vẫn đang coi:
- `FoodStall.Name`
- `FoodStall.Description`
- `Tour.Name`
- `Tour.Description`
là field trực tiếp

## 10.2 TTS sai nguồn dữ liệu
Sai nếu app:
- vẫn tìm `TtsScript`
- hoặc chưa đọc từ `Description`

## 10.3 QR flow chưa đúng
Sai nếu app:
- scan ra text nhưng không gọi API resolve
- hardcode mapping QR trong app

## 10.4 Tour chưa sort đúng
Sai nếu app:
- không sort `TourItems` theo `OrderIndex`

## 10.5 PlaybackLog chưa có
Sai nếu app:
- phát nội dung nhưng không gửi log

---

# 11. Checklist ngắn để review app

## FoodStall
- [ ] App đã dùng Translation chưa
- [ ] App có đọc `Description` để phát chưa

## GPS
- [ ] Có trigger theo `Radius`
- [ ] Có chống phát lặp

## QR
- [ ] Có scan ra text
- [ ] Có gọi QR resolve API
- [ ] Có dùng response từ backend

## PlaybackLog
- [ ] Có gửi log sau khi phát
- [ ] Có phân biệt GPS và QR

## Tour
- [ ] Có TourTranslation
- [ ] Có sort theo `OrderIndex`

---

# 12. Kết luận

Backend và Web Admin hiện tại đã hỗ trợ gần đầy đủ cho v1.

Để app làm việc đúng với hệ thống, app cần:
1. dùng đúng model đa ngôn ngữ
2. đọc `Description` để phát TTS
3. gọi QR resolve API khi scan QR
4. ghi PlaybackLog sau mỗi lần phát
5. hiểu Tour theo:
   - Tour
   - TourTranslation
   - TourItem
   - OrderIndex

Phần cần ưu tiên tích hợp nhất hiện tại là:
- **QR**
- **PlaybackLog**
- **Tour**
vì đây là 3 khu vực dễ lệch nhất giữa app và backend.