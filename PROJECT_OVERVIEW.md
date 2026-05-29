# TheOldPavement — Project Overview

Ứng dụng Web thương mại điện tử chuyên về thời trang **Premium Streetwear / Local Brand**, xây dựng trên **ASP.NET Core 9.0** với kiến trúc phân lớp (Clean Architecture).

---

## 🏗 Kiến trúc Hệ thống

```
TheOldPavement
├── Domain          → Models, Interfaces, Constants  (không phụ thuộc layer nào)
├── Application     → Services, DTOs, Validators, Mappings  (→ Domain)
├── Infrastructure  → EF Core DbContext, Repositories  (→ Domain)
└── Web             → Razor Pages, Program.cs  (→ Application + Infrastructure)
```

### 1. Domain
Trung tâm của hệ thống, không phụ thuộc layer nào.

**Models:**
`Cart`, `CartItem`, `Collaboration`, `CollaborationProduct`, `Collection`, `CollectionDetail`,
`CommercialProject`, `NewsletterSubscriber`, `Notification`, `Order`, `OrderItem`,
`Product`, `ProductDetail`, `ProductImage`, `ProductReview`, `ProductVariant`, `ProductView`,
`ProjectDeliverable`, `ProjectProduct`, `PromoCode`, `Promotion`, `Review`, `Sale`, `SaleProduct`,
`ShippingAddress`, `SizeChart`, `Store`, `User`, `UserAddress`, `Wishlist`, `AddToCartEvent`

**Interfaces:** `IRepository<T>`, `IProductRepository`, `IOrderRepository`, `IUserRepository`

**Constants:** `AppSettings`, `ErrorMessages`, `ValidationMessages`

---

### 2. Application
Chứa toàn bộ business logic.

**Services:**
| Service | Interface |
|---------|-----------|
| `AuthService` | `IAuthService` |
| `CheckoutService` | `ICheckoutService` |
| `EmailService` | `IEmailService` |
| `MomoService` | `IMomoService` |
| `OrderService` | `IOrderService` |
| `ProductService` | `IProductService` |
| `UserService` | `IUserService` |

**Khác:** DTOs, AutoMapper `MappingProfile`, FluentValidation Validators, `MomoOptions`

---

### 3. Infrastructure
Tầng truy cập dữ liệu.

**DbContext:** `TheOldPavementDbContext` (MySQL via Pomelo EF Core)

**Repositories:**
| Repository | Interface |
|------------|-----------|
| `Repository<T>` | `IRepository<T>` (generic) |
| `ProductRepository` | `IProductRepository` |
| `OrderRepository` | `IOrderRepository` |
| `UserRepository` | `IUserRepository` |

---

### 4. Web
Giao diện người dùng với Razor Pages + Tailwind CSS.

**Shared Partials:** `_Layout`, `_Header`, `_Footer`, `_CartDrawer`, `_ProductCard`, `_Toast`, `_Pagination`

---

## 🚀 Tính năng & Giao diện

### Public (Khách hàng — truy cập tự do)

| Route | Trang |
|-------|-------|
| `/` | Trang chủ — Landing page giới thiệu brand |
| `/Public/Shop` | Cửa hàng — Danh sách sản phẩm, filter, search |
| `/Product/Detail/{slug}` | Chi tiết sản phẩm — Gallery, chọn size/màu, thêm giỏ |
| `/Cart` | Giỏ hàng — Quản lý items, điều chỉnh số lượng |
| `/Checkout` | Thanh toán — Thông tin giao hàng, phương thức thanh toán |
| `/ThankYouCard` | Xác nhận đơn hàng thành công |
| `/Wishlist` | Danh sách yêu thích + Recently Viewed + Gợi ý sản phẩm |
| `/Sale` | Sản phẩm đang giảm giá |
| `/Outlet` | Xả hàng cuối mùa — chiết khấu sâu, không đổi trả |
| `/Collabs` | Các bộ sưu tập hợp tác |
| `/Collections` | Danh sách bộ sưu tập |
| `/CollectionDetail` | Chi tiết bộ sưu tập |
| `/RollingStones` | Campaign đặc biệt phong cách rock/vintage |
| `/CommercialProject` | Các dự án thương mại của brand |
| `/Lookbook` | Lookbook |
| `/About` | Về chúng tôi |
| `/Privacy` | Chính sách bảo mật |
| `/Error` | Trang lỗi thân thiện |

**Authentication:**
| Route | Chức năng |
|-------|-----------|
| `/Public/Account/Login` | Đăng nhập |
| `/Public/Account/Register` | Đăng ký |
| `/Public/Account/Logout` | Đăng xuất |
| `/Public/Account/ForgotPassword` | Quên mật khẩu |

**Payment:**
| Route | Chức năng |
|-------|-----------|
| `/Public/Payment/BankTransfer` | Thanh toán chuyển khoản ngân hàng |
| `/Public/Payment/MomoCallback` | Callback từ MoMo sau thanh toán |
| `/Public/Payment/MomoSimulate` | Simulate MoMo (dev/test) |

---

### Customer (Thành viên đã đăng nhập)

| Route | Chức năng |
|-------|-----------|
| `/Customer/Profile` | Xem/cập nhật thông tin cá nhân, đổi mật khẩu |
| `/Customer/Orders` | Lịch sử đơn hàng, Order Timeline, hủy đơn (khi `Pending`) |

---

### Admin (Quản trị hệ thống)

| Route | Chức năng |
|-------|-----------|
| `/Admin/Dashboard` | Bảng điều khiển trung tâm |

**Dashboard bao gồm:**
- Analytics: biểu đồ doanh thu tuần, thống kê COD vs Chuyển khoản, top sản phẩm bán chạy, KPI tổng (doanh thu, đơn hàng, sản phẩm, khách hàng)
- Cảnh báo tồn kho thấp (biến thể ≤ 15 sản phẩm)
- Quản lý Sản phẩm: thêm/sửa/xóa, ảnh, biến thể size/màu
- Quản lý Tồn kho: cập nhật nhanh số lượng từng biến thể
- Quản lý Mã khuyến mãi: thêm/sửa/vô hiệu hóa PromoCode (% hoặc cố định, giới hạn lượt dùng, đơn tối thiểu)

---

## ⚙️ Hệ thống Core & Tích hợp

**Checkout & Inventory**
- `CheckoutService` xử lý toàn bộ luồng đặt hàng
- Tự động kiểm tra và trừ `StockQuantity` của `ProductVariant` khi đặt hàng
- Dynamic Shipping Fee: tính phí theo Tỉnh/Thành phố, miễn phí ship khi đơn ≥ 500.000đ
- Guest Checkout: tự động tạo tài khoản cho khách chưa đăng ký

**Thanh toán**
- MoMo API (HttpClient named `"MoMo"`, strongly-typed `MomoOptions`)
- Chuyển khoản ngân hàng (BankTransfer)
- `payment_status`: `pending` | `paid` | `failed` | `awaiting_confirmation` | `refunded`

**Authentication**
- Cookie-based (`TheOldPavementAuth`)
- Session: 30 phút idle (`TheOldPavementSession`)
- Roles: Admin / Customer

**Khuyến mãi**
- `PromoCode`: kiểm tra backend — số lần dùng, thời hạn, giá trị đơn tối thiểu, loại % hoặc cố định

**Email**
- `EmailService` gửi thông báo đơn hàng và xác nhận đăng ký

**Sản phẩm liên quan & Gợi ý**
- Related Products trên trang chi tiết sản phẩm
- Recently Viewed + Suggested Products trên trang Wishlist

---

## 🛠 Công nghệ sử dụng

| Hạng mục | Công nghệ |
|----------|-----------|
| **Runtime** | .NET 9.0 |
| **Web Framework** | ASP.NET Core Razor Pages |
| **ORM** | Entity Framework Core 8.0.6 |
| **Database** | MySQL (Pomelo.EntityFrameworkCore.MySql 8.0.2) |
| **Mapping** | AutoMapper 13.0.1 |
| **Validation** | FluentValidation 11.9.2 |
| **Logging** | Serilog.AspNetCore 8.0.3 |
| **Payment** | MoMo API |
| **Frontend** | Tailwind CSS (CDN), Vanilla JS, jQuery |
| **Icons** | Lucide Icons |
| **Auth** | ASP.NET Core Cookie Authentication |
