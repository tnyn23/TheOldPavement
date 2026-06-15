<div align="center">
  <img src="https://via.placeholder.com/150x150/3E503C/F3ECDB?text=TOP" alt="The Old Pavement Logo" width="120" />
  <h1>The Old Pavement</h1>
  <p><strong>Nền tảng Thương mại Điện tử Phong cách Streetwear Tối giản & Hiện đại</strong></p>
</div>

<hr/>

## 🌟 Giới thiệu

**The Old Pavement** là một dự án E-commerce chuyên nghiệp được xây dựng trên nền tảng **ASP.NET Core 9.0**. Dự án mang đậm phong cách streetwear đương đại lấy cảm hứng từ các thương hiệu thời trang cao cấp như Represent, Fear Of God, Acne Studios; tập trung vào trải nghiệm người dùng (UX) tối ưu và giao diện (UI) tối giản, cao cấp. Hệ thống được thiết kế theo chuẩn **Clean Architecture**, đảm bảo khả năng mở rộng, dễ dàng bảo trì và tích hợp sâu các dịch vụ hiện đại.

---

## 🚀 Công nghệ sử dụng

### Backend & Cấu trúc
- **Framework**: ASP.NET Core 9.0 (Razor Pages)
- **Kiến trúc**: Clean Architecture (Domain, Application, Infrastructure, Web)
- **Database**: MySQL 8.0+
- **ORM**: Entity Framework Core (Pomelo.EntityFrameworkCore.MySql)

### Tích hợp Dịch vụ
- **Thanh toán MoMo**: Cổng thanh toán MoMo (tích hợp IPN callback, simulate sandbox)
- **Thanh toán Chuyển khoản Ngân hàng**: MBBank, polling tự động kiểm tra trạng thái
- **COD**: Thanh toán khi nhận hàng
- **Gửi Email**: SMTP Server (Gmail) — xác nhận đơn hàng tự động
- **Xác thực**: Cookie-based Authentication (Role: `user`, `admin`)

### Giao diện & Trải nghiệm
- **Thiết kế**: UI/UX cao cấp, Responsive trên mọi thiết bị, micro-animations
- **Màu sắc thương hiệu**:
  - 🌲 *Forest Green (#3E503C)*
  - 🍂 *Orange (#FF6F3D)*
  - 🍦 *Cream (#F3ECDB)*

### Triển khai (DevOps)
- **Containerization**: Docker (Dockerfile tích hợp sẵn)
- **CI/CD**: GitHub Actions (thư mục `.github/`)
- **Hosting / Deployment**: Railway (cấu hình `railway.toml`), VPS, hoặc Docker Swarm.

---

## 📋 Yêu cầu hệ thống

Để chạy dự án trên máy cá nhân (Local Environment), bạn cần cài đặt:
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MySQL Server 8.0+](https://dev.mysql.com/downloads/)
- (Tùy chọn) Docker Desktop nếu muốn chạy qua Container.

---

## ⚙️ Hướng dẫn Cài đặt & Khởi chạy

### 1. Clone repository

```bash
git clone https://github.com/tnyn23/TheOldPavement.git
cd TheOldPavement
```

### 2. Cấu hình Môi trường

Tạo file `src/Web/appsettings.Development.json` (File này đã được `.gitignore` để bảo mật) dựa trên template sau:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=the_old_pavement;User Id=root;Password=your_password;CharSet=utf8mb4;"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password"
  },
  "Momo": {
    "PartnerCode": "YOUR_MOMO_PARTNER_CODE",
    "AccessKey": "YOUR_MOMO_ACCESS_KEY",
    "SecretKey": "YOUR_MOMO_SECRET_KEY",
    "ApiUrl": "https://test-payment.momo.vn/v2/gateway/api/create"
  }
}
```

### 3. Cập nhật Database (Migration)

Dự án sử dụng EF Core. Bạn có thể sử dụng lệnh Migration để tạo cấu trúc cơ sở dữ liệu:

```bash
cd src/Web
dotnet ef database update --project ../Infrastructure/Infrastructure.csproj --startup-project Web.csproj
```
*(Hoặc import schema từ thư mục `/database/schema.sql` nếu có sẵn).*

### 4. Khởi chạy Ứng dụng

#### Cách 1: Chạy trực tiếp bằng .NET CLI
```bash
cd src/Web
dotnet run
```
Mở trình duyệt và truy cập: `http://localhost:5081` hoặc theo cổng mặc định hiển thị trên console.

#### Cách 2: Chạy bằng Docker
```bash
# Build image
docker build -t theoldpavement .

# Run container 
docker run -d -p 5081:8080 --name theoldpavement-app theoldpavement
```

---

## 📦 Cấu trúc Thư mục

Tuân thủ nghiêm ngặt **Clean Architecture**, dự án được chia thành các Layer tách biệt, giúp source code luôn rõ ràng:

```text
TheOldPavement/
├── src/
│   ├── Domain/              # Core layer: Entities, Enums, Value Objects, Interfaces
│   ├── Application/         # Use cases, Interfaces, DTOs, Services, MappingProfile, Validators
│   ├── Infrastructure/      # Data Access: DbContext, Repositories, External Services
│   └── Web/                 # Presentation: Razor Pages, Controllers, wwwroot (CSS/JS)
│       └── Pages/
│           ├── Admin/       # Trang quản trị (Dashboard)
│           ├── Customer/    # Trang cá nhân khách hàng (Profile, Orders, Notifications)
│           ├── Product/     # Trang chi tiết sản phẩm
│           ├── Public/      # Trang công khai (Account, Payment)
│           └── ...          # Index, Shop, Cart, Checkout, Wishlist, v.v.
├── Dockerfile               # Tệp cấu hình Docker build
├── railway.toml             # Cấu hình triển khai Railway
└── README.md                # Tài liệu dự án
```

---

## 📝 Chức năng Nổi bật (Features)

### 🛍️ Trải nghiệm Mua sắm (Storefront)

- ✅ **Trang chủ (Homepage)**: Hero section premium, hiển thị sản phẩm nổi bật, lookbook preview, và các bộ sưu tập.
- ✅ **Catalog Sản phẩm (Shop)**: Hiển thị sản phẩm dạng thẻ hiện đại, phân trang (12 sp/trang), bộ lọc đa chiều (danh mục, giá min/max, sắp xếp), tìm kiếm thông minh với **từ đồng nghĩa Việt–Anh** (vd: "áo thun" ↔ "tee").
- ✅ **Gợi ý Tìm kiếm (Search Suggestions)**: Autocomplete realtime khi gõ tên sản phẩm.
- ✅ **Chi tiết Sản phẩm**: Gallery ảnh theo màu sắc (color-variant images), chọn size/màu tương tác, thông tin variant (tồn kho, SKU), sản phẩm liên quan ngẫu nhiên cùng danh mục.
- ✅ **Lookbook**: Trang editorial với hotspot tương tác liên kết thẳng đến trang sản phẩm.
- ✅ **Bộ sưu tập (Collections)**: Hiển thị các collection theo mùa (season/year), liên kết sản phẩm theo collection.
- ✅ **Sale Page**: Trang sản phẩm khuyến mãi, hỗ trợ quick-add vào giỏ hàng.
- ✅ **Outlet**: Trang hàng thanh lý/outlet với lọc theo tình trạng sản phẩm.
- ✅ **Collab Page**: Trang collaboration với các thương hiệu/nghệ sĩ (Rolling Stones, v.v.).
- ✅ **Commercial Project**: Trang thương mại/dự án thương hiệu.
- ✅ **About & Privacy**: Trang giới thiệu thương hiệu và chính sách.

### 🛒 Giỏ hàng & Thanh toán

- ✅ **Giỏ hàng (Cart)**: Thêm/xóa/cập nhật số lượng, tính tổng tiền realtime, lưu trữ trong Session.
- ✅ **Danh sách Yêu thích (Wishlist)**: Toggle thêm/xóa yêu thích, chuyển sang giỏ hàng trực tiếp, hiển thị sản phẩm gần đây xem & đề xuất sản phẩm ngẫu nhiên.
- ✅ **Checkout đa phương thức thanh toán**: COD, chuyển khoản ngân hàng, MoMo — tự động điền thông tin từ tài khoản đăng nhập.
- ✅ **Mã Khuyến mãi (Promo Code)**: Áp dụng coupon khi checkout, kiểm tra hạn sử dụng, giới hạn lượt dùng, đơn tối thiểu.
- ✅ **Thanh toán MoMo**: Tạo link thanh toán qua API MoMo, xử lý IPN callback phía server, sandbox simulate khi dev.
- ✅ **Thanh toán Ngân hàng (MBBank)**: Hiển thị QR/thông tin tài khoản, **polling tự động** kiểm tra trạng thái thanh toán (JS call mỗi vài giây), admin xác nhận từ Dashboard.
- ✅ **Thank You Card**: Trang xác nhận đơn hàng sau khi thanh toán thành công.

### 👤 Tài khoản Khách hàng

- ✅ **Đăng ký / Đăng nhập / Đăng xuất**: Cookie-based Authentication với phân quyền `user` / `admin`.
- ✅ **Quên mật khẩu**: Gửi mật khẩu mới qua email (chống email enumeration attack).
- ✅ **Hồ sơ cá nhân (Profile)**: Xem và cập nhật tên, số điện thoại; đổi mật khẩu có xác thực mật khẩu cũ.
- ✅ **Lịch sử Đơn hàng (Customer Orders)**: Xem danh sách và trạng thái các đơn hàng, **hủy đơn hàng** đang chờ xử lý (tự động hoàn lại tồn kho).
- ✅ **Thông báo (Notifications)**: Xem danh sách thông báo hệ thống, tự động đánh dấu đã đọc khi mở trang.
- ✅ **Hệ thống Hạng Thành viên (Tier)**: Ghi nhận tổng chi tiêu (`total_spent`) và hạng thành viên (`Standard`, v.v.) cho mỗi user.

### ⭐ Đánh giá Sản phẩm

- ✅ **Gửi đánh giá (Review)**: Chỉ khách hàng đã mua và nhận hàng mới được đánh giá (xác thực qua `IReviewService.CanUserReviewProductAsync`).
- ✅ **Upload ảnh kèm review**: Tải lên tối đa nhiều ảnh kèm theo đánh giá, lưu tại `/uploads/reviews/`.
- ✅ **Rating Filter**: Lọc đánh giá theo số sao (1–5), hiển thị phân phối % từng mức sao.
- ✅ **Đánh dấu Hữu ích (Mark Helpful)**: Người dùng có thể đánh dấu review hữu ích.
- ✅ **Trung bình sao (Average Rating)**: Tính toán tự động, hiển thị tỉ lệ phân phối từng mức sao.

### 🔧 Trang Quản trị (Admin Dashboard)

- ✅ **Tổng quan (Analytics)**: Tổng doanh thu, tổng đơn hàng, tổng sản phẩm, tổng khách hàng; biểu đồ doanh thu theo ngày trong tuần (Mon–Sun); Top 3 sản phẩm bán chạy; tỉ lệ kênh thanh toán (COD vs. Chuyển khoản).
- ✅ **Quản lý Sản phẩm**: Thêm mới / chỉnh sửa / ẩn (soft delete) sản phẩm; upload tối đa 3 ảnh/sản phẩm theo từng màu sắc; quản lý trạng thái (`available`, `sold_out`, `coming_soon`, `discontinued`, `hidden`); flag `isFeatured`, `isOnSale`, `isCollab`, `isLimitedEdition` với thông tin tương ứng.
- ✅ **Quản lý Đơn hàng**: Xem danh sách đơn hàng, cập nhật trạng thái đơn hàng (`pending`, `confirmed`, `shipped`, `delivered`, v.v.), xác nhận thanh toán ngân hàng và gửi email xác nhận.
- ✅ **Quản lý Tồn kho (Inventory)**: Cập nhật số lượng tồn kho theo từng variant (size/màu) trực tiếp từ dashboard.
- ✅ **Quản lý Mã Khuyến mãi (Promo Codes)**: Tạo, chỉnh sửa, vô hiệu hóa mã giảm giá; hỗ trợ loại `percentage` và `fixed`; thiết lập giá trị đơn tối thiểu, giới hạn lượt dùng, ngày hiệu lực.
- ✅ **Kiểm duyệt Đánh giá (Reviews)**: Duyệt/ẩn và xóa đánh giá sản phẩm từ admin.
- ✅ **Bảo mật Truy cập Admin**: Chỉ role `admin` mới vào được, tự redirect về trang Login nếu không có quyền.

### 📧 Tính năng Email & Thông báo

- ✅ **Email Xác nhận Đơn hàng**: Tự động gửi sau khi admin xác nhận thanh toán ngân hàng hoặc MoMo callback thành công.
- ✅ **Email Quên mật khẩu**: Gửi mật khẩu mới tạm thời qua email.
- ✅ **Notification Service**: `INotificationService` tạo và quản lý thông báo in-app cho user.
- ✅ **Notification Dispatcher**: `INotificationDispatcher` phân phối thông báo hệ thống.

### 🏪 Mô hình Dữ liệu (Domain)

Dự án có đầy đủ các entity chuyên nghiệp:

| Entity | Mô tả |
|--------|-------|
| `Product` | Sản phẩm với đầy đủ flags: featured, sale, collab, limited edition, outlet |
| `ProductVariant` | Variant theo Size & Color với SKU, tồn kho riêng |
| `ProductImage` | Đa ảnh theo màu (AltText = color key) |
| `ProductReview` | Đánh giá với rating, ảnh đính kèm |
| `ProductView` | Theo dõi lượt xem sản phẩm |
| `Order` / `OrderItem` | Đơn hàng với đầy đủ trạng thái và thanh toán |
| `ShippingAddress` | Địa chỉ giao hàng đính kèm đơn hàng |
| `PromoCode` | Mã giảm giá với validation phức tạp |
| `Cart` / `CartItem` | Giỏ hàng persistent (database-backed) |
| `Wishlist` | Danh sách yêu thích |
| `Collection` | Bộ sưu tập theo mùa |
| `Collaboration` | Thông tin collab thương hiệu |
| `Sale` / `SaleProduct` | Chương trình sale theo sự kiện |
| `Notification` | Thông báo in-app |
| `User` | Tài khoản với tier, total_spent |
| `UserAddress` | Sổ địa chỉ người dùng |
| `AddToCartEvent` | Theo dõi hành vi thêm vào giỏ |
| `Store` | Thông tin cửa hàng |

---

## 🔐 Cảnh báo Bảo mật (Security Guidelines)

- Tuyệt đối **KHÔNG** commit các file chứa thông tin nhạy cảm như Mật khẩu, Connection String, API Keys (`appsettings.Development.json`) lên public repository.
- Sử dụng **App Password** thay vì mật khẩu Gmail thật.
- Đối với môi trường Production (như Railway), hãy khai báo thông qua **Environment Variables** thay vì viết cứng trong file cấu hình.
- Trang Admin được bảo vệ bởi role-based authorization — chỉ `admin` mới truy cập được.
- Chức năng đánh giá sản phẩm được bảo vệ — chỉ khách hàng đã mua & nhận hàng mới gửi được.

---

## 🤝 Đóng góp (Contributing)

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit thay đổi (`git commit -m 'Add some AmazingFeature'`)
4. Push lên branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request để được review.

---

## 👥 Đội ngũ Phát triển

Dự án được xây dựng trong khuôn khổ **Đồ án môn học EXE201 - FPT University**.
- **Môn học**: Khởi nghiệp (Entrepreneurship)
- **Email Liên hệ**: [theoldpavement@gmail.com](mailto:theoldpavement@gmail.com)


<br/>
<p align="center">Made with ❤️ by <strong>The Old Pavement Team</strong></p>
