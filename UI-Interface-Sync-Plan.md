# UI Interface Synchronization Plan

## Goal
Đồng bộ giao diện và cấu trúc trình bày cho tất cả trang Razor trong `src/Web/Pages`, sử dụng chung `Shared/_Layout.cshtml`, `Shared/_Header.cshtml` và `Shared/_Footer.cshtml` để đảm bảo thiết kế nhất quán, dễ bảo trì và tương thích responsive.

## Scope
Trang cần đồng bộ giao diện:

- `Index.cshtml`
- `Shop.cshtml`
- `Sale.cshtml`
- `RollingStones.cshtml`
- `Collabs.cshtml`
- `Outlet.cshtml`
- `CommercialProject.cshtml`
- `About.cshtml`
- `Privacy.cshtml`
- `Cart.cshtml`
- `Checkout.cshtml`
- `Wishlist.cshtml`
- `ThankYouCard.cshtml`
- `Error.cshtml`
- `Customer/Orders.cshtml`
- `Admin/Dashboard.cshtml`
- `Product/Detail.cshtml`
- `Public/Account/Login.cshtml`
- `Public/Account/Register.cshtml`
- `Public/Account/Logout.cshtml`
- `Public/Account/ForgotPassword.cshtml`
- `Public/Payment/MomoSimulate.cshtml`
- `Public/Payment/MomoCallback.cshtml`

## Current strong points

- `src/Web/Pages/Shared/_Layout.cshtml` đã có chung header/footer và dùng Tailwind CDN.
- `Shop.cshtml` đã có cấu trúc `main` với spacing chuẩn, và dùng `@section Styles` / `Scripts` cho các trang cần style riêng.
- `Shared/_Header.cshtml` đã xử lý menu di động và dropdown, có thể dùng làm giao diện chuẩn cho tất cả trang.

## Problems cần giải quyết

1. Một số trang chưa dùng layout/chưa có cấu trúc wrapper đồng nhất.
2. Bộ lọc và thanh công cụ trên trang `Shop` cần chuẩn lại để dùng cùng hệ thống spacing và typography với các trang khác.
3. Các trang con như `Account`, `Payment`, `Checkout`, `Cart` có thể thiếu cấu trúc `main`/padding/typography nhất quán.
4. Các tệp CSS hiện tại có thể cần chuẩn hóa để dùng chung màu nền, font và gutters.
5. Cần xác định các trang có script riêng để giữ `@section Styles`/`@section Scripts` mà không phá vỡ layout chung.

## Proposed procedure

1. **Đánh giá và chuẩn bị**
   - Xác định trang nào dùng đúng `Layout`, trang nào chưa.
   - Xác định trang nào cần `section Styles`/`section Scripts` riêng.
   - Kiểm tra script và stylesheet riêng tồn tại trong `wwwroot/js` và `wwwroot/css`.

2. **Chuẩn hóa layout chung**
   - Đảm bảo `Shared/_Layout.cshtml` chứa:
     - `meta viewport`
     - common CSS
     - `@RenderSection("Styles", required: false)`
     - `@RenderBody()` trong container chuẩn
     - `Shared/_Header` và `Shared/_Footer`
     - `@await RenderSectionAsync("Scripts", required: false)`
   - Thêm body class và spacing mặc định.

3. **Chuẩn hóa header/footer**
   - Đồng bộ các liên kết chính và định nghĩa style cho desktop/mobile.
   - Đảm bảo icon, label và accessibility attributes.
   - Dùng cùng header/footer cho toàn bộ trang.

4. **Đồng bộ schema trang public**
   - Áp các quy tắc `main` chung:
     - `pt-24` để tránh header cố định.
     - `px-4 md:px-8 lg:px-16` và `max-w-[1400px] mx-auto`.
     - `pb-16` hoặc `pb-20` cho footer.
   - Đối với trang `Shop`, `Product/Detail`, `Cart`, `Checkout`, `Wishlist`, `ThankYouCard`, `Error`, `About`, `CommercialProject`, `Outlet`, `Collabs`, `Sale`, `RollingStones`:
     - Đồng bộ typography, button style, card spacing.

5. **Áp dụng cho các trang quản lý & tài khoản**
   - `Customer/Orders.cshtml`
   - `Public/Account/*`
   - `Admin/Dashboard.cshtml`
   - `Public/Payment/*`
   - Giữ cấu trúc đồng nhất với phần public nhưng có thể dùng layout riêng nếu cần thiết cho admin.

6. **Kiểm tra CSS/JS**
   - Dùng các file `theme.css`, `product-card.css`, `site.css`, `site.js`, `product-card.js` để đồng bộ.
   - Nếu cần, thêm CSS riêng vào `theme.css` hoặc tạo file mới chung cho form, card, button.

7. **Kiểm tra và xác nhận**
   - Build solution.
   - Chạy manual review các trang sau khi áp dụng.
   - Sửa lỗi layout/responsive, kiểm tra header menu, tính năng cart/wishlist.

## Next action

- [ ] Xác nhận kế hoạch này
- [ ] Bắt đầu cập nhật `Shared/_Layout.cshtml` và `Shared/_Header.cshtml`
- [ ] Chuyển sang điều chỉnh từng nhóm trang (public -> account/payment -> admin)

## Ghi chú

- Nếu bạn muốn, tôi sẽ giữ phần `Admin` và `Account` riêng một bước sau khi hoàn thiện phần `Public` để tránh thay đổi quá lớn cùng lúc.
- Tôi sẽ không sửa trang nào trước khi bạn xác nhận kế hoạch này.