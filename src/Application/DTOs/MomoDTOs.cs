using System.Text.Json.Serialization;

namespace Application.DTOs;

// ─────────────────────────────────────────────────────────────────────────────
// REQUEST — sent to MoMo API
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Payload sent to MoMo /v2/gateway/api/create endpoint.
/// Field order in signature string must be alphabetical by key name.
/// </summary>
public sealed class MomoCreatePaymentRequest
{
    [JsonPropertyName("partnerCode")]
    public string PartnerCode { get; set; } = string.Empty;

    [JsonPropertyName("partnerName")]
    public string PartnerName { get; set; } = string.Empty;

    [JsonPropertyName("storeId")]
    public string StoreId { get; set; } = string.Empty;

    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    [JsonPropertyName("orderId")]
    public string OrderId { get; set; } = string.Empty;

    [JsonPropertyName("orderInfo")]
    public string OrderInfo { get; set; } = string.Empty;

    [JsonPropertyName("redirectUrl")]
    public string RedirectUrl { get; set; } = string.Empty;

    [JsonPropertyName("ipnUrl")]
    public string IpnUrl { get; set; } = string.Empty;

    [JsonPropertyName("lang")]
    public string Lang { get; set; } = "vi";

    [JsonPropertyName("extraData")]
    public string ExtraData { get; set; } = string.Empty;

    [JsonPropertyName("requestType")]
    public string RequestType { get; set; } = "captureWallet";

    [JsonPropertyName("signature")]
    public string Signature { get; set; } = string.Empty;
}

// ─────────────────────────────────────────────────────────────────────────────
// RESPONSE — received from MoMo API after create-payment call
// ─────────────────────────────────────────────────────────────────────────────

public sealed class MomoCreatePaymentResponse
{
    [JsonPropertyName("partnerCode")]
    public string PartnerCode { get; set; } = string.Empty;

    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("orderId")]
    public string OrderId { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    [JsonPropertyName("responseTime")]
    public long ResponseTime { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>0 = success, anything else = error</summary>
    [JsonPropertyName("resultCode")]
    public int ResultCode { get; set; }

    /// <summary>Full payment URL to redirect user to</summary>
    [JsonPropertyName("payUrl")]
    public string PayUrl { get; set; } = string.Empty;

    /// <summary>Short link (QR deep link)</summary>
    [JsonPropertyName("shortLink")]
    public string ShortLink { get; set; } = string.Empty;

    [JsonPropertyName("qrCodeUrl")]
    public string QrCodeUrl { get; set; } = string.Empty;

    public bool IsSuccess => ResultCode == 0 && !string.IsNullOrEmpty(PayUrl);
}

// ─────────────────────────────────────────────────────────────────────────────
// CALLBACK — query params MoMo sends back to returnUrl AND ipnUrl
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Parameters MoMo appends to both returnUrl (browser redirect) and
/// ipnUrl (server-to-server POST/GET notification).
/// </summary>
public sealed class MomoCallbackParams
{
    public string PartnerCode { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string OrderInfo { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public string TransId { get; set; } = string.Empty;
    public string ResultCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string PayType { get; set; } = string.Empty;
    public string ResponseTime { get; set; } = string.Empty;
    public string ExtraData { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;

    /// <summary>resultCode == "0" means success</summary>
    public bool IsSuccess => ResultCode == "0";
}

// ─────────────────────────────────────────────────────────────────────────────
// SERVICE RESULT — internal DTO returned by IMomoService
// ─────────────────────────────────────────────────────────────────────────────

public sealed class MomoPaymentResult
{
    public bool Success { get; set; }
    public string PayUrl { get; set; } = string.Empty;
    public string QrCodeUrl { get; set; } = string.Empty;
    public string ShortLink { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public int ResultCode { get; set; }
}
