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
*   **Trang cá nhân (`/Customer/Profile`)**: Xem và cập nhật thông tin cá nhân (Họ tên, Số điện thoại) và đổi mật khẩu an toàn.
*   **Lịch sử đơn hàng (`/Customer/Orders`)**: 
    *   Xem lịch sử mua sắm chi tiết.
    *   **Theo dõi hành trình đơn hàng (Order Timeline)**: Trực quan hóa trạng thái đơn hàng (Đặt hàng → Vận chuyển → Giao hàng).
    *   **Hủy đơn hàng**: Cho phép khách hàng tự hủy đơn hàng khi trạng thái còn là `Pending` (Hệ thống sẽ tự động hoàn lại tồn kho tương ứng cho các biến thể sản phẩm).

### 3. Giao diện Admin (Quản trị hệ thống)
Khu vực quản lý dành cho nhân viên/quản trị viên.
*   **Dashboard (`/Admin/Dashboard`)**: Bảng điều khiển trung tâm đa chức năng.
    *   **Analytics / Overview**: Biểu đồ doanh thu tuần, thống kê kênh thanh toán chính (COD vs Chuyển khoản), các sản phẩm bán chạy nhất, và bộ chỉ số tổng (Doanh thu, Đơn hàng, Sản phẩm, Khách hàng).
    *   **Cảnh báo hết hàng**: Hiển thị danh sách cảnh báo tồn kho thấp (các biến thể sản phẩm có số lượng $\le 15$) trực tiếp tại trang tổng quan để quản trị viên kịp thời nhập thêm hàng.
    *   Quản lý Sản phẩm (Thêm/Sửa/Xóa sản phẩm, ảnh, biến thể size, màu sắc).
    *   Quản lý Tồn kho (Cập nhật nhanh số lượng tồn kho của từng biến thể sản phẩm).
    *   Quản lý Mã khuyến mãi (Thêm/Sửa/Vô hiệu hóa code giảm giá theo loại phần trăm hoặc cố định, giới hạn đơn tối thiểu và lượt dùng).

### 4. Hệ thống Core & Tích hợp
*   **Sản phẩm liên quan (Related Products)**: Tự động đề xuất các sản phẩm cùng loại trên trang chi tiết sản phẩm.
*   **Sản phẩm đã xem & Đề xuất (Wishlist Page)**:
    *   **Recently Viewed**: Ghi nhận và hiển thị các sản phẩm khách hàng vừa xem trong phiên làm việc.
    *   **Gợi ý sản phẩm**: Thuật toán tự chọn gợi ý các sản phẩm phù hợp trên trang danh sách yêu thích.
*   **Mua hàng & Tồn kho (Checkout & Inventory)**: Quản lý luồng đặt hàng chuyên biệt qua `CheckoutService`, tự động kiểm tra và trừ tồn kho (StockQuantity) của biến thể sản phẩm khi đặt hàng. Dynamic Shipping Fee tự động tính phí giao hàng dựa trên Tỉnh/Thành phố và chính sách miễn phí ship (hóa đơn $\ge 500k$).
*   **Thanh toán**: Tích hợp API cổng thanh toán Momo.
*   **Email**: Gửi email thông báo đơn hàng/đăng ký qua `EmailService`.
*   **Xác thực (Authentication)**: Đăng nhập, đăng ký, quên mật khẩu qua hệ thống Identity/AuthService. Hỗ trợ tự động tạo tài khoản (Guest Checkout) cho khách chưa có tài khoản.
*   **Khuyến mãi**: Hệ thống mã giảm giá (PromoCode) tính toán bảo mật tại Backend, kiểm tra chặt chẽ số lần dùng, thời hạn và giá trị đơn tối thiểu.

---

## 🛠 Công nghệ sử dụng
*   **Backend**: .NET 9.0, Entity Framework Core.
*   **Frontend**: Razor Pages, HTML5/CSS3 (Tailwind CSS cho Styling & Layout), Vanilla JS (Alpine/jQuery tuỳ ngữ cảnh).
*   **Payment**: Momo API.
*   **Database**: SQL Server (thông qua EF Core Migrations).
*   **Icons**: Lucide Icons.
