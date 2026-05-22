# TheOldPavement - Project Overview

Dự án này là một ứng dụng Web (có vẻ là một trang thương mại điện tử hoặc giới thiệu sản phẩm/dự án) được xây dựng trên nền tảng **ASP.NET Core** với kiến trúc phân lớp (Layered Architecture).

## 🏗 Kiến trúc Hệ thống

Dự án được chia thành các Layer chính sau:

1.  **TheOldPavement.Core**:
    *   Chứa các **Entities (Models)**: Cart, Product, User, Order, Collaboration, v.v.
    *   Các interface cơ bản và các hằng số (Constants).
    *   Đây là trung tâm của dự án, không phụ thuộc vào các lớp khác.

2.  **TheOldPavement.Application**:
    *   Chứa logic nghiệp vụ (Business Logic).
    *   **Services**: Xử lý logic cho Product, Auth, Email và tích hợp thanh toán (Momo).
    *   **DTOs & Mappings**: Chuyển đổi dữ liệu giữa Core và Web.
    *   **Validators**: Kiểm tra tính hợp lệ của dữ liệu đầu vào.

3.  **TheOldPavement.Data**:
    *   Chứa **DbContext** (Entity Framework Core).
    *   **Repositories**: Các lớp truy xuất dữ liệu từ Database.
    *   **Configurations & Seeds**: Cấu hình bảng và dữ liệu mẫu.

4.  **TheOldPavement.Web**:
    *   Sử dụng **Razor Pages** cho giao diện người dùng.
    *   Chứa các Controller/PageModel xử lý yêu cầu từ trình duyệt.
    *   Tích hợp Middleware và Helpers.

---

## 🚀 Các tính năng hiện có (Features List)

Dựa trên cấu trúc file, dự án hiện có các tính năng sau:

### 1. Quản lý Sản phẩm & Cửa hàng
*   **Danh mục sản phẩm**: Xem danh sách sản phẩm, chi tiết sản phẩm (`ProductDetail`).
*   **Biến thể sản phẩm**: Hỗ trợ màu sắc, kích thước (`ProductVariant`, `SizeChart`).
*   **Hình ảnh & Đánh giá**: Quản lý ảnh sản phẩm và review từ khách hàng.
*   **Tìm kiếm & Lọc**: Theo loại, bộ sưu tập (`Collection`).

### 2. Mua hàng & Giỏ hàng
*   **Giỏ hàng (Cart)**: Thêm/Xóa sản phẩm, cập nhật số lượng.
*   **Wishlist**: Lưu sản phẩm yêu thích.
*   **Thanh toán (Checkout)**: Quy trình đặt hàng, nhập địa chỉ giao hàng.
*   **Tích hợp thanh toán**: Đã có tích hợp với **Momo Service**.

### 3. Thành viên & Bảo mật
*   **Xác thực (Authentication)**: Đăng nhập, đăng ký thông qua `AuthService`.
*   **Quản lý người dùng**: Thông tin cá nhân, địa chỉ nhận hàng (`UserAddress`).
*   **Phân quyền**: Có khu vực dành riêng cho Admin (`Pages/Admin`).

### 4. Marketing & Truyền thông
*   **Khuyến mãi**: Mã giảm giá (`PromoCode`), các chương trình Sale.
*   **Newsletter**: Đăng ký nhận bản tin qua email.
*   **Dự án & Hợp tác**: Quản lý các dự án thương mại (`CommercialProject`) và cộng tác (`Collaboration`).

### 5. Hệ thống hỗ trợ
*   **Email**: Gửi thông báo qua `EmailService`.
*   **Thông báo (Notification)**: Hệ thống thông báo trong ứng dụng.

---

## 🛠 Công nghệ sử dụng
*   **Backend**: .NET Core, Entity Framework Core.
*   **Frontend**: Razor Pages, HTML/CSS/JS.
*   **Payment**: Momo API.
*   **Database**: (Dựa trên EF Core, thường là SQL Server).
