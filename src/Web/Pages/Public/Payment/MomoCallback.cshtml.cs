using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Web.Helpers;

namespace Web.Pages.Public.Payment;

/// <summary>
/// Handles two distinct MoMo callbacks:
///
///   1. returnUrl  — browser redirect after user finishes on MoMo page.
///      The user CAN tamper with query params here, so we ONLY read the
///      result from the database (already updated by IPN).
///
///   2. ipnUrl (IPN) — server-to-server GET/POST from MoMo servers.
///      This is the authoritative source. We verify signature and update DB.
///      Must respond HTTP 204 (no content) quickly so MoMo doesn't retry.
///
/// Security: signature is verified on EVERY request before any DB write.
/// </summary>
public sealed class MomoCallbackModel : PageModel
{
    private readonly IMomoService _momoService;
    private readonly IEmailService _emailService;
    private readonly ILogger<MomoCallbackModel> _logger;

    public bool IsSuccess { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public string OrderNumber { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public bool IsIpn { get; private set; }

    public MomoCallbackModel(
        IMomoService momoService,
        IEmailService emailService,
        ILogger<MomoCallbackModel> logger)
    {
        _momoService  = momoService;
        _emailService = emailService;
        _logger       = logger;
    }

    /// <summary>
    /// Handles both returnUrl (browser) and ipnUrl (server-to-server).
    /// Distinguish via ?ipn=1 query param appended to ipnUrl.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(
        [FromQuery] string partnerCode,
        [FromQuery] string orderId,
        [FromQuery] string requestId,
        [FromQuery] string amount,
        [FromQuery] string orderInfo,
        [FromQuery] string orderType,
        [FromQuery] string transId,
        [FromQuery] string resultCode,
        [FromQuery] string message,
        [FromQuery] string payType,
        [FromQuery] string responseTime,
        [FromQuery] string extraData,
        [FromQuery] string signature,
        [FromQuery] bool ipn = false,
        CancellationToken ct = default)
    {
        IsIpn = ipn;

        var callback = new MomoCallbackParams
        {
            PartnerCode  = partnerCode  ?? string.Empty,
            OrderId      = orderId      ?? string.Empty,
            RequestId    = requestId    ?? string.Empty,
            Amount       = amount       ?? string.Empty,
            OrderInfo    = orderInfo    ?? string.Empty,
            OrderType    = orderType    ?? string.Empty,
            TransId      = transId      ?? string.Empty,
            ResultCode   = resultCode   ?? string.Empty,
            Message      = message      ?? string.Empty,
            PayType      = payType      ?? string.Empty,
            ResponseTime = responseTime ?? string.Empty,
            ExtraData    = extraData    ?? string.Empty,
            Signature    = signature    ?? string.Empty
        };

        _logger.LogInformation(
            "MoMo callback received. OrderId={OrderId} ResultCode={ResultCode} IsIPN={IsIpn}",
            callback.OrderId, callback.ResultCode, ipn);

        // ── 1. Verify signature — reject anything that doesn't match ─────────
        if (!_momoService.VerifyCallbackSignature(callback))
        {
            _logger.LogWarning(
                "MoMo signature verification FAILED for order {OrderId}. " +
                "Possible tampering attempt. IP: {IP}",
                callback.OrderId,
                HttpContext.Connection.RemoteIpAddress);

            if (ipn)
            {
                // IPN: return 400 so MoMo knows we rejected it
                return StatusCode(400, "Invalid signature");
            }

            IsSuccess   = false;
            OrderNumber = callback.OrderId;
            Message     = "Chữ ký bảo mật không hợp lệ. Giao dịch có thể đã bị can thiệp.";
            return Page();
        }

        // ── 2. Process: update DB ────────────────────────────────────────────
        var success = await _momoService.ProcessCallbackAsync(callback, ct);

        if (ipn)
        {
            // IPN must respond quickly with 204 — no body needed
            return StatusCode(204);
        }

        // ── 3. returnUrl: show result page ───────────────────────────────────
        IsSuccess   = success;
        OrderNumber = callback.OrderId;

        if (success)
        {
            // Clear cart only on confirmed success
            CartManager.ClearCart(HttpContext.Session);

            if (decimal.TryParse(callback.Amount, out var amt))
                Amount = amt;

            Message = "Thanh toán thành công! Cảm ơn bạn đã mua hàng tại The Old Pavement.";

            _logger.LogInformation(
                "MoMo returnUrl: payment SUCCESS for order {OrderId}", callback.OrderId);
        }
        else
        {
            Message = $"Giao dịch không thành công hoặc đã bị hủy. (Mã MoMo: {callback.ResultCode} — {callback.Message})";

            _logger.LogWarning(
                "MoMo returnUrl: payment FAILED for order {OrderId}: [{Code}] {Msg}",
                callback.OrderId, callback.ResultCode, callback.Message);
        }

        return Page();
    }
}
