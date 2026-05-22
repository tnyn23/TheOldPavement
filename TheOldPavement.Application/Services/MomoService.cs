using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TheOldPavement.Application.Interfaces;
using TheOldPavement.Domain.Models;

namespace TheOldPavement.Application.Services;

public class MomoService : IMomoService
{
    private readonly IConfiguration _configuration;

    public MomoService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> CreatePaymentUrlAsync(Order order, string redirectUrl, string ipnUrl)
    {
        var partnerCode = _configuration["Momo:PartnerCode"] ?? "MOMOBKUN20180529";
        var accessKey = _configuration["Momo:AccessKey"] ?? "klm05TvNBzhg7h7j";
        var secretKey = _configuration["Momo:SecretKey"] ?? "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";
        var apiUrl = _configuration["Momo:ApiUrl"] ?? "https://test-payment.momo.vn/v2/gateway/api/create";

        var requestId = Guid.NewGuid().ToString();
        var orderInfo = $"Thanh toan don hang {order.OrderNumber} qua MoMo";
        var amount = ((long)Math.Round(order.TotalAmount)).ToString(); // MoMo expects a string representable integer in VND
        var extraData = ""; // optional
        var requestType = "captureWallet";

        // Create raw data string exactly as required by MoMo API v2
        var rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={order.OrderNumber}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";

        var signature = ComputeHmacSha256(rawHash, secretKey);

        // Build the payload
        var payload = new
        {
            partnerCode,
            partnerName = "The Old Pavement Store",
            storeId = "TheOldPavement",
            requestId,
            amount = long.Parse(amount),
            orderId = order.OrderNumber,
            orderInfo,
            redirectUrl,
            ipnUrl,
            lang = "vi",
            extraData,
            requestType,
            signature
        };

        using var client = new HttpClient();
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(apiUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"MoMo API returned status code {response.StatusCode}: {responseString}");
            }

            var momoResponse = JsonSerializer.Deserialize<MomoCreateResponse>(responseString);
            if (momoResponse == null)
            {
                throw new Exception("Failed to deserialize MoMo API response.");
            }

            if (momoResponse.ResultCode != 0)
            {
                throw new Exception($"MoMo payment creation failed. ResultCode: {momoResponse.ResultCode}, Message: {momoResponse.Message}");
            }

            if (string.IsNullOrEmpty(momoResponse.PayUrl))
            {
                throw new Exception("MoMo API response did not contain a PayUrl.");
            }

            return momoResponse.PayUrl;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error during MoMo payment creation: {ex.Message}", ex);
        }
    }

    public bool ValidateSignature(
        string partnerCode, 
        string orderId, 
        string requestId, 
        string amount, 
        string orderInfo, 
        string orderType, 
        string transId, 
        string resultCode, 
        string message, 
        string payType, 
        string responseTime, 
        string extraData, 
        string signature)
    {
        var secretKey = _configuration["Momo:SecretKey"] ?? "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";

        // MoMo v2 Callback raw signature string format:
        // accessKey=$accessKey&amount=$amount&extraData=$extraData&message=$message&orderId=$orderId&orderInfo=$orderInfo&partnerCode=$partnerCode&paymentCode=$paymentCode&payType=$payType&requestId=$requestId&responseTime=$responseTime&resultCode=$resultCode&transId=$transId
        // Note: The paymentCode is optional or not present in redirect callbacks.
        // Let's verify standard MoMo Callback raw signature layout.
        // Usually, the raw callback string is:
        // accessKey={accessKey}&amount={amount}&extraData={extraData}&message={message}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&payType={payType}&requestId={requestId}&responseTime={responseTime}&resultCode={resultCode}&transId={transId}
        // Let's build it carefully.
        
        var accessKey = _configuration["Momo:AccessKey"] ?? "klm05TvNBzhg7h7j";
        
        var rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&message={message}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&payType={payType}&requestId={requestId}&responseTime={responseTime}&resultCode={resultCode}&transId={transId}";
        
        var calculatedSignature = ComputeHmacSha256(rawHash, secretKey);

        return string.Equals(calculatedSignature, signature, StringComparison.OrdinalIgnoreCase);
    }

    private string ComputeHmacSha256(string message, string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(messageBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}

public class MomoCreateResponse
{
    [JsonPropertyName("partnerCode")]
    public string PartnerCode { get; set; } = string.Empty;

    [JsonPropertyName("orderId")]
    public string OrderId { get; set; } = string.Empty;

    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    [JsonPropertyName("responseTime")]
    public long ResponseTime { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("resultCode")]
    public int ResultCode { get; set; }

    [JsonPropertyName("payUrl")]
    public string PayUrl { get; set; } = string.Empty;

    [JsonPropertyName("shortLink")]
    public string ShortLink { get; set; } = string.Empty;
}

