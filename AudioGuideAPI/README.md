# AudioGuideAPI (Backend)

## 1. Giới thiệu

Đây là backend của hệ thống Audio Guide.

Chức năng chính:
- Lưu trữ dữ liệu các địa điểm (POI - Point of Interest)
- Cung cấp API cho ứng dụng mobile (MAUI)
- Hỗ trợ thêm / sửa / xoá dữ liệu

---

## 2. Công nghệ sử dụng

- ASP.NET Core Web API
- Entity Framework Core
- SQLite

---

## 3. Cách chạy project (Setup)

### Bước 1: Mở project

- Mở project bằng Visual Studio
- Chọn project `AudioGuideAPI`

---

### Bước 2: Tạo database

Mở:

```
Tools → NuGet Package Manager → Package Manager Console
```

Chạy lệnh:

```powershell
Update-Database
```

Lệnh này sẽ:
- tạo database SQLite
- tạo bảng `POIs`

---

### Bước 3: Chạy backend

- Bấm Run (https) trong Visual Studio

---

### Bước 4: Mở Swagger

Mở trình duyệt:

```
https://localhost:7246/swagger
```

(Lưu ý: port có thể khác tùy máy)

----------------

## 4. Test API

### GET /api/Poi

Lấy danh sách POI

---

### POST /api/Poi

Thêm POI mới

Body (JSON):

```json
{
  "name": "Cổng vào phố ẩm thực Vĩnh Khánh",
  "latitude": 10.75510,
  "longitude": 106.70380,
  "radius": 30,
  "description": "Điểm bắt đầu của khu phố ẩm thực.",
  "audioUrl": "audio/poi1.mp3"
}
```

---

### GET /api/Poi/{id}

Lấy thông tin POI theo id

Ví dụ:

```
GET /api/Poi/1
```

---

### PUT /api/Poi/{id}

Cập nhật POI

```json
{
  "id": 1,
  "name": "Updated",
  "latitude": 10.75510,
  "longitude": 106.70380,
  "radius": 30,
  "description": "Updated description",
  "audioUrl": "audio/test.mp3"
}
```

---

### DELETE /api/Poi/{id}

Xóa POI

```
DELETE /api/Poi/1
```

---

## 5. Lưu ý

- API POST chỉ nhận 1 object mỗi lần
- Không cần gửi id khi POST
- Database sẽ tự tạo id
- Nếu chưa có dữ liệu, hãy dùng POST để thêm

---

## 6. Cách hệ thống hoạt động

Mobile App (MAUI)  
→ gọi API Backend  
→ Backend truy vấn Database (SQLite)  
→ trả dữ liệu về app  

---

## 7. Dữ liệu

- Database nằm trong file `.db`
- Dữ liệu lưu local (không nằm trong Git)
- Mỗi người chạy project sẽ có database riêng

---

## 8. Trạng thái hiện tại

Backend đã hoàn thành:
- CRUD API (Create, Read, Update, Delete)
- Kết nối database SQLite
- Test API bằng Swagger

---

## 9. Ghi chú

Nếu gặp lỗi:
- kiểm tra đã chạy `Update-Database` chưa
- kiểm tra đúng port khi mở Swagger