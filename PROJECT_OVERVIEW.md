# TheOldPavement - Project Overview

Dự án này là một ứng dụng Web thương mại điện tử chuyên về thời trang (Premium Streetwear / Local Brand) được xây dựng trên nền tảng **ASP.NET Core** với kiến trúc phân lớp (Layered Architecture).

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

## 🚀 Tính năng & Giao diện hiện có (UI & Features Overview)

Hệ thống được chia thành 3 nhóm giao diện chính: **Public (Khách hàng)**, **Customer (Thành viên)**, và **Admin (Quản trị)**.

### 1. Giao diện Public (Khách hàng)
Các trang dành cho người dùng truy cập tự do, mua sắm và tìm hiểu thông tin thương hiệu.
*   **Trang chủ (`/`)**: Landing page giới thiệu brand.
*   **Cửa hàng (`/Shop`)**: Danh sách toàn bộ sản phẩm. Sử dụng UI Card chuẩn (`_ProductCard.cshtml`).
*   **Chi tiết sản phẩm (`/Product/{slug}`)**: Hiển thị thông tin chi tiết, hình ảnh (gallery), chọn size/màu và nút Thêm vào giỏ hàng.
*   **Bộ sưu tập / Chiến dịch**:
    *   **Collabs (`/Collabs`)**: Các bộ sưu tập hợp tác.
    *   **Rolling Stones (`/RollingStones`)**: Trang campaign đặc biệt mang phong cách rock/vintage.
    *   **Commercial Project (`/CommercialProject`)**: Các dự án thương mại của brand.
*   **Khuyến mãi & Xả hàng**:
    *   **Sale (`/Sale`)**: Danh sách sản phẩm đang giảm giá.
    *   **Outlet (`/Outlet`)**: Trang xả hàng cuối mùa (Clearance) với chiết khấu sâu (mặc định 40% trong code), điều khoản không đổi trả.
*   **Trải nghiệm mua sắm**:
    *   **Giỏ hàng (`/Cart`)**: Quản lý các sản phẩm đã thêm, điều chỉnh số lượng.
    *   **Thanh toán (`/Checkout`)**: Điền thông tin giao hàng, chọn phương thức thanh toán (hỗ trợ tích hợp Momo).
    *   **Thank You (`/ThankYouCard`)**: Trang xác nhận sau khi đặt hàng/thanh toán thành công.
    *   **Wishlist (`/Wishlist`)**: Danh sách sản phẩm yêu thích (lưu tạm hoặc theo user).
*   **Thông tin tĩnh**:
    *   **Về chúng tôi (`/About`)**: Lịch sử và thông điệp của The Old Pavement.
    *   **Bảo mật (`/Privacy`)**: Chính sách bảo mật thông tin.
    *   **Lỗi (`/Error`)**: Trang xử lý hiển thị lỗi thân thiện với người dùng.

### 2. Giao diện Customer (Thành viên)
Khu vực dành cho người dùng đã đăng nhập.
*   **Lịch sử đơn hàng (`/Customer/Orders`)**: Quản lý và theo dõi trạng thái các đơn hàng đã đặt của cá nhân khách hàng.
*   *(Các tính năng quản lý profile, địa chỉ, đổi mật khẩu đang được tích hợp qua Identity/AuthService).*

### 3. Giao diện Admin (Quản trị hệ thống)
Khu vực quản lý dành cho nhân viên/quản trị viên.
*   **Dashboard (`/Admin/Dashboard`)**: Bảng điều khiển trung tâm đa chức năng.
    *   Quản lý Sản phẩm (Thêm/Sửa/Xóa sản phẩm, ảnh, biến thể size).
    *   Quản lý Đơn hàng (Theo dõi doanh thu, trạng thái đơn hàng).
    *   Sử dụng modal form và các tab để quản lý trực tiếp trên một trang mà không cần reload.

### 4. Hệ thống Core & Tích hợp
*   **Mua hàng & Tồn kho (Checkout & Inventory)**: Quản lý luồng đặt hàng chuyên biệt qua `CheckoutService`, tự động kiểm tra và trừ tồn kho (StockQuantity) của biến thể sản phẩm khi đặt hàng. Kiến trúc tuân thủ tuyệt đối Clean Architecture (Web layer không tiếp xúc trực tiếp DbContext).
*   **Thanh toán**: Tích hợp API cổng thanh toán Momo.
*   **Email**: Gửi email thông báo đơn hàng/đăng ký qua `EmailService`.
*   **Xác thực (Authentication)**: Đăng nhập, đăng ký qua hệ thống Identity/AuthService. Hỗ trợ tự động tạo tài khoản (Guest Checkout) cho khách chưa có tài khoản.
*   **Khuyến mãi**: Hệ thống mã giảm giá (PromoCode) tính toán bảo mật tại Backend, kiểm tra chặt chẽ số lần dùng, thời hạn và giá trị đơn tối thiểu.

---

## 🛠 Công nghệ sử dụng
*   **Backend**: .NET 9.0, Entity Framework Core.
*   **Frontend**: Razor Pages, HTML5/CSS3 (Tailwind CSS cho Styling & Layout), Vanilla JS (Alpine/jQuery tuỳ ngữ cảnh).
*   **Payment**: Momo API.
*   **Database**: SQL Server (thông qua EF Core Migrations).
*   **Icons**: Lucide Icons.
