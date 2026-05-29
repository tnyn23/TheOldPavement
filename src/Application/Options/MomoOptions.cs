namespace Application.Options;

/// <summary>
/// Strongly-typed MoMo configuration bound from appsettings.json "Momo" section.
/// Never hardcode these values — always read from IOptions&lt;MomoOptions&gt;.
/// </summary>
public sealed class MomoOptions
{
    public const string SectionName = "Momo";

    /// <summary>Partner code provided by MoMo (e.g. MOMOBKUN20180529)</summary>
    public string PartnerCode { get; set; } = string.Empty;

    /// <summary>Access key provided by MoMo</summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>Secret key for HMAC-SHA256 signature — NEVER log this</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>MoMo create-payment API endpoint</summary>
    public string ApiUrl { get; set; } = "https://test-payment.momo.vn/v2/gateway/api/create";

    /// <summary>Request type: captureWallet | payWithMethod</summary>
    public string RequestType { get; set; } = "captureWallet";

    /// <summary>Store display name shown on MoMo payment page</summary>
    public string PartnerName { get; set; } = "The Old Pavement Store";

    /// <summary>Store ID shown on MoMo payment page</summary>
    public string StoreId { get; set; } = "TheOldPavement";
}
