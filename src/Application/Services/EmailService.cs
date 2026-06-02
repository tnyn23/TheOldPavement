using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Application.Interfaces;
using Domain.Models;

namespace Application.Services;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderPassword;
    private readonly string _fromEmail;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _smtpServer     = configuration["Email:SmtpServer"]   ?? "smtp.gmail.com";
        _smtpPort       = int.Parse(configuration["Email:SmtpPort"] ?? "587");
        _senderEmail    = configuration["Email:SenderEmail"]  ?? "";
        _senderPassword = configuration["Email:SenderPassword"] ?? "";
        // FromEmail: địa chỉ hiển thị cho người nhận (có thể khác SMTP login)
        _fromEmail      = configuration["Email:FromEmail"] ?? _senderEmail;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
    {
        if (string.IsNullOrWhiteSpace(_senderEmail) || string.IsNullOrWhiteSpace(_senderPassword))
        {
            _logger.LogWarning("[Email] SenderEmail hoặc SenderPassword chưa được cấu hình");
            return;
        }

        _logger.LogInformation("[Email] Đang gửi tới {To} | Subject: {Subject} | Server: {Server}:{Port}", 
            toEmail, subject, _smtpServer, _smtpPort);

        try
        {
            // Thử port 465 (SSL) trước, nếu config là 587 thì override
            var port = _smtpPort;
            var enableSsl = true;

            using var client = new SmtpClient(_smtpServer, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000
            };

            using var message = new MailMessage
            {
                From        = new MailAddress(_fromEmail, "The Old Pavement"),
                Subject     = subject,
                Body        = body,
                IsBodyHtml  = isHtml,
                BodyEncoding = Encoding.UTF8
            };

            message.To.Add(toEmail);

            await client.SendMailAsync(message);
            _logger.LogInformation("[Email] Gửi thành công tới {To}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Email] SMTP FAILED tới {To} | Server: {Server}:{Port} | Error: {Error}", 
                toEmail, _smtpServer, _smtpPort, ex.Message);
            throw;
        }
    }

    public async Task SendOrderConfirmationEmailAsync(Order order)
    {
        var address = order.ShippingAddress;
        if (address == null)
        {
            _logger.LogWarning("[Email] SendOrderConfirmationEmailAsync: ShippingAddress là null cho đơn hàng {OrderNumber}", order.OrderNumber);
            return;
        }

        _logger.LogInformation("[Email] Chuẩn bị gửi xác nhận đơn hàng {OrderNumber} tới {Email}", order.OrderNumber, address.Email);

        // Build order items rows
        var itemsHtml = new StringBuilder();
        foreach (var item in order.OrderItems)
        {
            itemsHtml.Append($@"
            <tr>
                <td style=""padding:12px 16px;border-bottom:1px solid #f0f0f0;"">
                    <strong style=""display:block;font-size:13px;color:#1a1a1a;text-transform:uppercase;letter-spacing:.5px;"">{item.ProductName}</strong>
                    <span style=""font-size:11px;color:#888;margin-top:2px;display:block;"">Màu: {item.Color?.ToUpper()} / Size: {item.Size?.ToUpper()} / SL: {item.Quantity}</span>
                </td>
                <td style=""padding:12px 16px;border-bottom:1px solid #f0f0f0;text-align:right;white-space:nowrap;"">
                    <span style=""font-size:13px;font-weight:700;color:#1a1a1a;"">{item.Subtotal:N0}₫</span>
                </td>
            </tr>");
        }

        var discountRow = order.DiscountAmount > 0
            ? $@"<tr>
                    <td style=""padding:8px 16px;color:#555;font-size:13px;"">Giảm giá</td>
                    <td style=""padding:8px 16px;text-align:right;color:#c0392b;font-size:13px;font-weight:600;"">-{order.DiscountAmount:N0}₫</td>
                 </tr>"
            : "";

        var paymentMethodText = order.PaymentMethod?.ToLower() switch
        {
            "cod"  => "Thanh toán khi nhận hàng (COD)",
            "momo" => "Ví điện tử MoMo — 0965481905 (Nguyễn Thế Hoàng Tùng)",
            "bank" => "Chuyển khoản MBBank — 0965481905 (Nguyen The Hoang Tung)",
            _      => "Thanh toán khi nhận hàng (COD)"
        };
        var fullAddress = $"{address.Address}, {address.Ward}, {address.District}, {address.City}".Replace(", ,", ",").Trim(',', ' ');

        var html = $@"
<!DOCTYPE html>
<html lang=""vi"">
<head><meta charset=""UTF-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""></head>
<body style=""margin:0;padding:0;background:#f5f5f5;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f5f5f5;padding:40px 0;"">
    <tr><td align=""center"">
      <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#fff;max-width:600px;width:100%;"">

        <!-- Header -->
        <tr>
          <td style=""background:#0a0a0a;padding:36px 40px;text-align:center;"">
            <h1 style=""margin:0;color:#fff;font-size:22px;letter-spacing:6px;font-weight:300;text-transform:uppercase;"">THE OLD PAVEMENT</h1>
            <p style=""margin:8px 0 0;color:#888;font-size:11px;letter-spacing:2px;text-transform:uppercase;"">Xác nhận đơn hàng</p>
          </td>
        </tr>

        <!-- Order info banner -->
        <tr>
          <td style=""background:#f8f8f8;padding:20px 40px;border-bottom:1px solid #eee;"">
            <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
              <tr>
                <td>
                  <p style=""margin:0;font-size:12px;color:#888;text-transform:uppercase;letter-spacing:1px;"">Mã đơn hàng</p>
                  <p style=""margin:4px 0 0;font-size:16px;font-weight:700;color:#0a0a0a;letter-spacing:1px;"">#{order.OrderNumber}</p>
                </td>
                <td align=""right"">
                  <p style=""margin:0;font-size:12px;color:#888;text-transform:uppercase;letter-spacing:1px;"">Ngày đặt hàng</p>
                  <p style=""margin:4px 0 0;font-size:14px;font-weight:600;color:#0a0a0a;"">{order.CreatedAt:dd/MM/yyyy HH:mm}</p>
                </td>
              </tr>
            </table>
          </td>
        </tr>

        <!-- Greeting -->
        <tr>
          <td style=""padding:32px 40px 16px;"">
            <h2 style=""margin:0 0 8px;font-size:18px;color:#0a0a0a;font-weight:700;"">Cảm ơn bạn, {address.FullName}! 🖤</h2>
            <p style=""margin:0;font-size:14px;color:#555;line-height:1.6;"">Đơn hàng của bạn đã được nhận thành công và đang trong quá trình xử lý. Chúng tôi sẽ thông báo cho bạn khi đơn hàng được gửi đi.</p>
          </td>
        </tr>

        <!-- Items Table -->
        <tr>
          <td style=""padding:0 40px 16px;"">
            <p style=""margin:0 0 12px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:2px;color:#888;"">Chi tiết đơn hàng</p>
            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border:1px solid #eee;"">
              <thead>
                <tr style=""background:#f8f8f8;"">
                  <th style=""padding:10px 16px;text-align:left;font-size:11px;text-transform:uppercase;letter-spacing:1px;color:#888;font-weight:600;"">Sản phẩm</th>
                  <th style=""padding:10px 16px;text-align:right;font-size:11px;text-transform:uppercase;letter-spacing:1px;color:#888;font-weight:600;"">Thành tiền</th>
                </tr>
              </thead>
              <tbody>
                {itemsHtml}
              </tbody>
            </table>
          </td>
        </tr>

        <!-- Price Summary -->
        <tr>
          <td style=""padding:0 40px 32px;"">
            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border:1px solid #eee;border-top:none;"">
              <tr>
                <td style=""padding:8px 16px;color:#555;font-size:13px;"">Tạm tính</td>
                <td style=""padding:8px 16px;text-align:right;font-size:13px;color:#1a1a1a;font-weight:600;"">{order.Subtotal:N0}₫</td>
              </tr>
              {discountRow}
              <tr>
                <td style=""padding:8px 16px;color:#555;font-size:13px;"">Phí vận chuyển</td>
                <td style=""padding:8px 16px;text-align:right;font-size:13px;color:#27ae60;font-weight:600;"">Miễn phí</td>
              </tr>
              <tr style=""background:#0a0a0a;"">
                <td style=""padding:14px 16px;color:#fff;font-size:14px;font-weight:700;text-transform:uppercase;letter-spacing:1px;"">Tổng cộng</td>
                <td style=""padding:14px 16px;text-align:right;font-size:16px;font-weight:700;color:#fff;"">{order.TotalAmount:N0}₫</td>
              </tr>
            </table>
          </td>
        </tr>

        <!-- Shipping + Payment Info -->
        <tr>
          <td style=""padding:0 40px 32px;"">
            <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
              <tr>
                <td width=""50%"" valign=""top"" style=""padding-right:16px;"">
                  <p style=""margin:0 0 8px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:2px;color:#888;"">Địa chỉ giao hàng</p>
                  <p style=""margin:0;font-size:13px;color:#1a1a1a;line-height:1.7;"">{address.FullName}<br>{address.Phone}<br>{fullAddress}</p>
                </td>
                <td width=""50%"" valign=""top"" style=""padding-left:16px;border-left:1px solid #eee;"">
                  <p style=""margin:0 0 8px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:2px;color:#888;"">Phương thức thanh toán</p>
                  <p style=""margin:0;font-size:13px;color:#1a1a1a;line-height:1.7;"">{paymentMethodText}</p>
                </td>
              </tr>
            </table>
          </td>
        </tr>

        <!-- Footer -->
        <tr>
          <td style=""background:#0a0a0a;padding:24px 40px;text-align:center;"">
            <p style=""margin:0 0 6px;font-size:11px;color:#666;letter-spacing:1px;"">© 2024 THE OLD PAVEMENT. All rights reserved.</p>
            <p style=""margin:0;font-size:11px;color:#555;"">Email này được gửi tự động, vui lòng không trả lời trực tiếp.</p>
          </td>
        </tr>

      </table>
    </td></tr>
  </table>
</body>
</html>";

        await SendEmailAsync(
            address.Email,
            $"[The Old Pavement] Xác nhận đơn hàng #{order.OrderNumber}",
            html
        );
    }

    public async Task SendAccountCreationEmailAsync(string toEmail, string fullName, string rawPassword)
    {
        var html = $@"
<!DOCTYPE html>
<html lang=""vi"">
<head><meta charset=""UTF-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""></head>
<body style=""margin:0;padding:0;background:#f5f5f5;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f5f5f5;padding:40px 0;"">
    <tr><td align=""center"">
      <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#fff;max-width:600px;width:100%;"">

        <!-- Header -->
        <tr>
          <td style=""background:#0a0a0a;padding:36px 40px;text-align:center;"">
            <h1 style=""margin:0;color:#fff;font-size:22px;letter-spacing:6px;font-weight:300;text-transform:uppercase;"">THE OLD PAVEMENT</h1>
            <p style=""margin:8px 0 0;color:#888;font-size:11px;letter-spacing:2px;text-transform:uppercase;"">Tài khoản của bạn</p>
          </td>
        </tr>

        <!-- Body -->
        <tr>
          <td style=""padding:40px 40px 16px;"">
            <h2 style=""margin:0 0 16px;font-size:20px;color:#0a0a0a;font-weight:700;"">Chào mừng, {fullName}! 🖤</h2>
            <p style=""margin:0 0 20px;font-size:14px;color:#555;line-height:1.8;"">
              Chúng tôi đã tự động tạo một tài khoản cho bạn để bạn có thể dễ dàng theo dõi các đơn hàng và lịch sử mua sắm của mình.
            </p>
            <p style=""margin:0 0 8px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:2px;color:#888;"">Thông tin đăng nhập</p>
            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f8f8f8;border:1px solid #eee;"">
              <tr>
                <td style=""padding:14px 20px;border-bottom:1px solid #eee;"">
                  <span style=""font-size:11px;color:#888;text-transform:uppercase;letter-spacing:1px;"">Email</span>
                  <strong style=""display:block;margin-top:4px;font-size:14px;color:#0a0a0a;"">{toEmail}</strong>
                </td>
              </tr>
              <tr>
                <td style=""padding:14px 20px;"">
                  <span style=""font-size:11px;color:#888;text-transform:uppercase;letter-spacing:1px;"">Mật khẩu</span>
                  <strong style=""display:block;margin-top:4px;font-size:18px;color:#0a0a0a;letter-spacing:3px;font-family:monospace;"">{rawPassword}</strong>
                </td>
              </tr>
            </table>
            <p style=""margin:20px 0 0;font-size:13px;color:#888;line-height:1.6;"">
              ⚠️ Để bảo mật tài khoản, vui lòng đăng nhập và <strong>đổi mật khẩu</strong> ngay sau khi nhận được email này.
            </p>
          </td>
        </tr>

        <!-- CTA -->
        <tr>
          <td style=""padding:24px 40px 40px;"">
            <a href=""/Public/Account/Login"" style=""display:inline-block;background:#0a0a0a;color:#fff;padding:14px 32px;font-size:12px;font-weight:700;text-decoration:none;text-transform:uppercase;letter-spacing:2px;"">Đăng nhập ngay →</a>
          </td>
        </tr>

        <!-- Footer -->
        <tr>
          <td style=""background:#0a0a0a;padding:24px 40px;text-align:center;"">
            <p style=""margin:0 0 6px;font-size:11px;color:#666;letter-spacing:1px;"">© 2024 THE OLD PAVEMENT. All rights reserved.</p>
            <p style=""margin:0;font-size:11px;color:#555;"">Email này được gửi tự động, vui lòng không trả lời trực tiếp.</p>
          </td>
        </tr>

      </table>
    </td></tr>
  </table>
</body>
</html>";

        await SendEmailAsync(
            toEmail,
            "[The Old Pavement] Tài khoản của bạn đã được tạo",
            html
        );
    }

    public async Task SendPasswordRecoveryEmailAsync(string toEmail, string fullName, string tempPassword, string loginUrl = "")
    {
        if (string.IsNullOrEmpty(loginUrl))
            loginUrl = "/Public/Account/Login";
        var html = $@"
<!DOCTYPE html>
<html lang=""vi"">
<head><meta charset=""UTF-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""></head>
<body style=""margin:0;padding:0;background:#f5f5f5;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f5f5f5;padding:40px 0;"">
    <tr><td align=""center"">
      <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#fff;max-width:600px;width:100%;"">

        <!-- Header -->
        <tr>
          <td style=""background:#0a0a0a;padding:36px 40px;text-align:center;"">
            <h1 style=""margin:0;color:#fff;font-size:22px;letter-spacing:6px;font-weight:300;text-transform:uppercase;"">THE OLD PAVEMENT</h1>
            <p style=""margin:8px 0 0;color:#888;font-size:11px;letter-spacing:2px;text-transform:uppercase;"">Khôi phục mật khẩu</p>
          </td>
        </tr>

        <!-- Body -->
        <tr>
          <td style=""padding:40px 40px 16px;"">
            <h2 style=""margin:0 0 16px;font-size:20px;color:#0a0a0a;font-weight:700;"">Xin chào, {fullName}</h2>
            <p style=""margin:0 0 24px;font-size:14px;color:#555;line-height:1.8;"">
              Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Dưới đây là mật khẩu tạm thời mới:
            </p>

            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f8f8f8;border:1px solid #eee;margin-bottom:20px;"">
              <tr>
                <td style=""padding:20px;text-align:center;"">
                  <p style=""margin:0 0 6px;font-size:11px;color:#888;text-transform:uppercase;letter-spacing:2px;"">Mật khẩu tạm thời</p>
                  <strong style=""display:block;font-size:24px;color:#0a0a0a;letter-spacing:4px;font-family:monospace;"">{tempPassword}</strong>
                </td>
              </tr>
            </table>

            <p style=""margin:0;font-size:13px;color:#888;line-height:1.6;"">
              ⚠️ Mật khẩu này có giá trị sử dụng ngay lập tức. Vui lòng đăng nhập và <strong>đổi mật khẩu</strong> ngay để bảo vệ tài khoản của bạn.
            </p>
            <p style=""margin:12px 0 0;font-size:13px;color:#888;line-height:1.6;"">
              Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này. Tài khoản của bạn vẫn an toàn.
            </p>
          </td>
        </tr>

        <!-- CTA -->
        <tr>
          <td style=""padding:24px 40px 40px;"">
            <a href=""{loginUrl}"" style=""display:inline-block;background:#0a0a0a;color:#fff;padding:14px 32px;font-size:12px;font-weight:700;text-decoration:none;text-transform:uppercase;letter-spacing:2px;"">Đăng nhập ngay →</a>
          </td>
        </tr>

        <!-- Footer -->
        <tr>
          <td style=""background:#0a0a0a;padding:24px 40px;text-align:center;"">
            <p style=""margin:0 0 6px;font-size:11px;color:#666;letter-spacing:1px;"">© 2024 THE OLD PAVEMENT. All rights reserved.</p>
            <p style=""margin:0;font-size:11px;color:#555;"">Email này được gửi tự động, vui lòng không trả lời trực tiếp.</p>
          </td>
        </tr>

      </table>
    </td></tr>
  </table>
</body>
</html>";

        await SendEmailAsync(
            toEmail,
            "[The Old Pavement] Mật khẩu mới của bạn",
            html
        );
    }
}


