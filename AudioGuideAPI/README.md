# 🎧 Audio Guide Backend (Vĩnh Khánh)

## 1. Giới thiệu

Đây là backend của hệ thống Audio Guide phục vụ cho ứng dụng mobile.

Chức năng chính:

* Cung cấp dữ liệu các địa điểm ẩm thực (Food Stall)
* Hỗ trợ đa ngôn ngữ (Việt / Anh)
* Trả dữ liệu cho mobile để:

  * hiển thị bản đồ
  * hiển thị thông tin địa điểm
  * phát thuyết minh (audio hoặc TTS)

---

## 2. Công nghệ sử dụng

* ASP.NET Core Web API
* Entity Framework Core
* SQLite
* Static Files (wwwroot)

---

## 3. Kiến trúc dữ liệu

Hệ thống sử dụng 3 bảng chính:

### Languages

* Ngôn ngữ hệ thống (vi, en)

### FoodStalls

* Thông tin chung:

  * tọa độ (lat, long)
  * bán kính (radius)
  * địa chỉ
  * ảnh
  * priority

### FoodStallTranslations

* Nội dung theo ngôn ngữ:

  * name
  * description
  * specialty
  * audioUrl

---

## 4. Cách chạy project

### Bước 1: Mở project

Mở bằng Visual Studio và chọn project `AudioGuideAPI`

---

### Bước 2: Chạy backend

Nhấn Run (https)

---

### Bước 3: Mở Swagger

```text
https://localhost:7246/swagger
```

(Lưu ý: port có thể khác tùy máy)

---

## 5. API chính

### 🔹 Lấy danh sách ngôn ngữ

```http
GET /api/Languages
```

---

### 🔹 Lấy danh sách địa điểm

```http
GET /api/FoodStalls?lang=vi
GET /api/FoodStalls?lang=en
```

---

### 🔹 Lấy chi tiết địa điểm

```http
GET /api/FoodStalls/{id}?lang=vi
```

---

## 6. Cấu trúc dữ liệu trả về

Ví dụ:

```json
{
  "id": 1,
  "name": "Quán Ốc Nướng Mỡ Hành",
  "address": "...",
  "specialty": "...",
  "priceRange": "...",
  "imageUrl": "https://localhost:7246/images/...",
  "latitude": 10.7599,
  "longitude": 106.7043,
  "radius": 35,
  "description": "...",
  "audioUrl": "https://localhost:7246/audio/vi/...",
  "priority": 1,
  "mapLink": "...",
  "languageCode": "vi"
}
```

---

## 7. Static Files

Backend phục vụ các file:

```text
/images/...
/audio/vi/...
/audio/en/...
/flags/...
```

---

## 8. Cách hệ thống hoạt động

Mobile App
→ gọi API backend
→ backend truy vấn SQLite
→ trả dữ liệu + URL ảnh/audio

App sẽ:

* hiển thị map từ lat/long
* kiểm tra khoảng cách (radius)
* phát audio hoặc dùng TTS từ description

---

## 9. Hỗ trợ TTS

Hiện tại hệ thống hỗ trợ 2 cách:

* 🔊 Phát audio từ `audioUrl` (nếu có)
* 🗣️ Dùng TTS đọc `description` (fallback)

---

## 10. Trạng thái hiện tại

Backend đã hoàn thành:

* kiến trúc dữ liệu mới
* đa ngôn ngữ (vi, en)
* API cho mobile
* static files (ảnh + audio)
* seed dữ liệu mẫu (5 địa điểm Vĩnh Khánh)

---

## 11. Lưu ý

* `localhost` chỉ dùng khi chạy cùng máy
* nếu chạy trên điện thoại/emulator:
  → cần đổi sang IP máy backend

---

## 12. Hướng phát triển

* upload ảnh/audio qua API
* admin CRUD
* thêm nhiều địa điểm
* triển khai production
