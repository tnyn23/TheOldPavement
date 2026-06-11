using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Application.Interfaces;
using Domain.Models;

namespace Application.Services;

public class EmailService : IEmailService
{
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _apiKey;
    private readonly string _smtpPassword;
    private readonly ILogger<EmailService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IHttpClientFactory httpClientFactory)
    {
        _fromEmail       = configuration["Email:FromEmail"] ?? configuration["Email:SenderEmail"] ?? "";
        _fromName        = "The Old Pavement";
        _apiKey          = configuration["Email:BrevoApiKey"] ?? "";
        _smtpPassword    = configuration["Email:SenderPassword"] ?? "";
        _logger          = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _logger.LogInformation("[Email] BrevoApiKey không tồn tại. Đang dùng SMTP Gmail fallback...");
            try
            {
                using var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(_fromEmail, _smtpPassword),
                    EnableSsl = true
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };
                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("[Email] Gửi SMTP Gmail thành công tới {To}", toEmail);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Email] Gửi SMTP Gmail thất bại: {Error}", ex.Message);
                return;
            }
        }

        _logger.LogInformation("[Email] Đang gửi qua Brevo API tới {To} | Subject: {Subject}", toEmail, subject);

        var payload = new
        {
            sender = new { email = _fromEmail, name = _fromName },
            to = new[] { new { email = toEmail } },
            subject = subject,
            htmlContent = isHtml ? body : null,
            textContent = isHtml ? null : body
        };

        var json = JsonSerializer.Serialize(payload);

        try
        {
            var client = _httpClientFactory.CreateClient("Brevo");
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
            request.Headers.Add("api-key", _apiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                _logger.LogInformation("[Email] Gửi Brevo thành công tới {To} | Status: {Status}", toEmail, response.StatusCode);
            else
                _logger.LogError("[Email] Brevo API lỗi | Status: {Status} | Body: {Body}", response.StatusCode, responseBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Email] HTTP request failed tới Brevo | Error: {Error}", ex.Message);
        }
    }

    public async Task SendOrderConfirmationEmailAsync(Order order)
    {
        var address = order.ShippingAddress;
        if (address == null)
        {
            _logger.LogWarning("[Email] ShippingAddress null cho đơn hàng {OrderNumber}", order.OrderNumber);
            return;
        }

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
            "momo" => "Ví điện tử MoMo — 0965481905 ",
            "bank" => "Chuyển khoản MBBank — 0965481905 ",
            _      => "Thanh toán khi nhận hàng (COD)"
        };
        var fullAddress = $"{address.Address}, {address.Ward}, {address.District}, {address.City}".Replace(", ,", ",").Trim(',', ' ');

        var html = $@"<!DOCTYPE html><html lang=""vi""><head><meta charset=""UTF-8""></head>
<body style=""margin:0;padding:0;background:#f5f5f5;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f5f5f5;padding:40px 0;"">
    <tr><td align=""center"">
      <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#fff;max-width:600px;width:100%;"">
        <tr><td style=""background:#0a0a0a;padding:36px 40px;text-align:center;"">
            <h1 style=""margin:0;color:#fff;font-size:22px;letter-spacing:6px;font-weight:300;text-transform:uppercase;"">THE OLD PAVEMENT</h1>
            <p style=""margin:8px 0 0;color:#888;font-size:11px;letter-spacing:2px;text-transform:uppercase;"">Xác nhận đơn hàng</p>
        </td></tr>
        <tr><td style=""background:#f8f8f8;padding:20px 40px;border-bottom:1px solid #eee;"">
            <table width=""100%""><tr>
                <td><p style=""margin:0;font-size:12px;color:#888;text-transform:uppercase;"">Mã đơn hàng</p>
                    <p style=""margin:4px 0 0;font-size:16px;font-weight:700;color:#0a0a0a;"">#{order.OrderNumber}</p></td>
                <td align=""right""><p style=""margin:0;font-size:12px;color:#888;text-transform:uppercase;"">Ngày đặt hàng</p>
                    <p style=""margin:4px 0 0;font-size:14px;font-weight:600;color:#0a0a0a;"">{order.CreatedAt:dd/MM/yyyy HH:mm}</p></td>
            </tr></table>
        </td></tr>
        <tr><td style=""padding:32px 40px 16px;"">
            <h2 style=""margin:0 0 8px;font-size:18px;color:#0a0a0a;font-weight:700;"">Cảm ơn bạn, {address.FullName}! 🖤</h2>
            <p style=""margin:0;font-size:14px;color:#555;line-height:1.6;"">Đơn hàng đã được nhận và đang xử lý.</p>
        </td></tr>
        <tr><td style=""padding:0 40px 16px;"">
            <table width=""100%"" style=""border:1px solid #eee;"">
              <thead><tr style=""background:#f8f8f8;"">
                <th style=""padding:10px 16px;text-align:left;font-size:11px;text-transform:uppercase;color:#888;"">Sản phẩm</th>
                <th style=""padding:10px 16px;text-align:right;font-size:11px;text-transform:uppercase;color:#888;"">Thành tiền</th>
              </tr></thead>
              <tbody>{itemsHtml}</tbody>
            </table>
        </td></tr>
        <tr><td style=""padding:0 40px 32px;"">
            <table width=""100%"" style=""border:1px solid #eee;border-top:none;"">
              <tr><td style=""padding:8px 16px;color:#555;font-size:13px;"">Tạm tính</td>
                  <td style=""padding:8px 16px;text-align:right;font-size:13px;font-weight:600;"">{order.Subtotal:N0}₫</td></tr>
              {discountRow}
              <tr><td style=""padding:8px 16px;color:#555;font-size:13px;"">Phí vận chuyển</td>
                  <td style=""padding:8px 16px;text-align:right;font-size:13px;color:#27ae60;font-weight:600;"">Miễn phí</td></tr>
              <tr style=""background:#0a0a0a;"">
                  <td style=""padding:14px 16px;color:#fff;font-size:14px;font-weight:700;text-transform:uppercase;"">Tổng cộng</td>
                  <td style=""padding:14px 16px;text-align:right;font-size:16px;font-weight:700;color:#fff;"">{order.TotalAmount:N0}₫</td>
              </tr>
            </table>
        </td></tr>
        <tr><td style=""padding:0 40px 32px;"">
            <table width=""100%""><tr>
                <td width=""50%"" valign=""top"" style=""padding-right:16px;"">
                    <p style=""margin:0 0 8px;font-size:11px;font-weight:700;text-transform:uppercase;color:#888;"">Địa chỉ giao hàng</p>
                    <p style=""margin:0;font-size:13px;color:#1a1a1a;line-height:1.7;"">{address.FullName}<br>{address.Phone}<br>{fullAddress}</p>
                </td>
                <td width=""50%"" valign=""top"" style=""padding-left:16px;border-left:1px solid #eee;"">
                    <p style=""margin:0 0 8px;font-size:11px;font-weight:700;text-transform:uppercase;color:#888;"">Phương thức thanh toán</p>
                    <p style=""margin:0;font-size:13px;color:#1a1a1a;line-height:1.7;"">{paymentMethodText}</p>
                </td>
            </tr></table>
        </td></tr>
        <tr><td style=""background:#0a0a0a;padding:24px 40px;text-align:center;"">
            <p style=""margin:0;font-size:11px;color:#666;"">© 2024 THE OLD PAVEMENT. All rights reserved.</p>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body></html>";

        await SendEmailAsync(address.Email, $"[The Old Pavement] Xác nhận đơn hàng #{order.OrderNumber}", html);
    }

    public async Task SendAccountCreationEmailAsync(string toEmail, string fullName, string rawPassword)
    {
        var html = $@"<!DOCTYPE html><html lang=""vi""><head><meta charset=""UTF-8""></head>
<body style=""margin:0;padding:0;background:#f5f5f5;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f5f5f5;padding:40px 0;"">
    <tr><td align=""center"">
      <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#fff;max-width:600px;width:100%;"">
        <tr><td style=""background:#0a0a0a;padding:36px 40px;text-align:center;"">
            <h1 style=""margin:0;color:#fff;font-size:22px;letter-spacing:6px;font-weight:300;text-transform:uppercase;"">THE OLD PAVEMENT</h1>
        </td></tr>
        <tr><td style=""padding:40px;"">
            <h2 style=""margin:0 0 16px;font-size:20px;color:#0a0a0a;"">Chào mừng, {fullName}! 🖤</h2>
            <p style=""margin:0 0 20px;font-size:14px;color:#555;line-height:1.8;"">Tài khoản của bạn đã được tạo thành công.</p>
            <table width=""100%"" style=""background:#f8f8f8;border:1px solid #eee;"">
              <tr><td style=""padding:14px 20px;border-bottom:1px solid #eee;"">
                  <span style=""font-size:11px;color:#888;text-transform:uppercase;"">Email</span>
                  <strong style=""display:block;margin-top:4px;font-size:14px;color:#0a0a0a;"">{toEmail}</strong>
              </td></tr>
              <tr><td style=""padding:14px 20px;"">
                  <span style=""font-size:11px;color:#888;text-transform:uppercase;"">Mật khẩu tạm thời</span>
                  <strong style=""display:block;margin-top:4px;font-size:18px;color:#0a0a0a;letter-spacing:3px;font-family:monospace;"">{rawPassword}</strong>
              </td></tr>
            </table>
            <p style=""margin:20px 0 0;font-size:13px;color:#888;"">⚠️ Vui lòng đổi mật khẩu sau khi đăng nhập.</p>
        </td></tr>
        <tr><td style=""background:#0a0a0a;padding:24px 40px;text-align:center;"">
            <p style=""margin:0;font-size:11px;color:#666;"">© 2024 THE OLD PAVEMENT. All rights reserved.</p>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body></html>";

        await SendEmailAsync(toEmail, "[The Old Pavement] Tài khoản của bạn đã được tạo", html);
    }

    public async Task SendPasswordRecoveryEmailAsync(string toEmail, string fullName, string tempPassword, string loginUrl = "")
    {
        var html = $@"<!DOCTYPE html><html lang=""vi""><head><meta charset=""UTF-8""></head>
<body style=""margin:0;padding:0;background:#f5f5f5;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f5f5f5;padding:40px 0;"">
    <tr><td align=""center"">
      <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#fff;max-width:600px;width:100%;"">
        <tr><td style=""background:#0a0a0a;padding:36px 40px;text-align:center;"">
            <h1 style=""margin:0;color:#fff;font-size:22px;letter-spacing:6px;font-weight:300;text-transform:uppercase;"">THE OLD PAVEMENT</h1>
            <p style=""margin:8px 0 0;color:#888;font-size:11px;letter-spacing:2px;text-transform:uppercase;"">Khôi phục mật khẩu</p>
        </td></tr>
        <tr><td style=""padding:40px;"">
            <h2 style=""margin:0 0 16px;font-size:20px;color:#0a0a0a;"">Xin chào, {fullName}</h2>
            <p style=""margin:0 0 24px;font-size:14px;color:#555;line-height:1.8;"">Mật khẩu tạm thời mới của bạn:</p>
            <table width=""100%"" style=""background:#f8f8f8;border:1px solid #eee;margin-bottom:20px;"">
              <tr><td style=""padding:20px;text-align:center;"">
                  <p style=""margin:0 0 6px;font-size:11px;color:#888;text-transform:uppercase;"">Mật khẩu tạm thời</p>
                  <strong style=""display:block;font-size:24px;color:#0a0a0a;letter-spacing:4px;font-family:monospace;"">{tempPassword}</strong>
              </td></tr>
            </table>
            <p style=""margin:0;font-size:13px;color:#888;line-height:1.6;"">⚠️ Vui lòng đăng nhập và <strong>đổi mật khẩu</strong> ngay để bảo vệ tài khoản.</p>
            <p style=""margin:12px 0 0;font-size:13px;color:#888;"">Nếu bạn không yêu cầu đặt lại, hãy bỏ qua email này.</p>
        </td></tr>
        <tr><td style=""background:#0a0a0a;padding:24px 40px;text-align:center;"">
            <p style=""margin:0;font-size:11px;color:#666;"">© 2024 THE OLD PAVEMENT. All rights reserved.</p>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body></html>";

        await SendEmailAsync(toEmail, "[The Old Pavement] Mật khẩu mới của bạn", html);
    }
}
