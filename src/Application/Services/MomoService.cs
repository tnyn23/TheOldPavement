using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Application.DTOs;
using Application.Interfaces;
using Application.Options;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class MomoService : IMomoService
{
    private readonly IConfiguration _configuration;
    private readonly IOrderRepository _orderRepository;

    public MomoService(IConfiguration configuration, IOrderRepository orderRepository)
    {
        _configuration = configuration;
        _orderRepository = orderRepository;
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private string PartnerCode => _configuration["Momo:PartnerCode"] ?? "MOMOBKUN20180529";
    private string AccessKey   => _configuration["Momo:AccessKey"]   ?? "klm05TvNBzhg7h7j";
    private string SecretKey   => _configuration["Momo:SecretKey"]   ?? "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";
    private string ApiUrl      => _configuration["Momo:ApiUrl"]      ?? "https://test-payment.momo.vn/v2/gateway/api/create";

    // ── CreatePaymentAsync ────────────────────────────────────────────────────

    public async Task<MomoPaymentResult> CreatePaymentAsync(
        Order order,
        string returnUrl,
        string ipnUrl,
        CancellationToken cancellationToken = default)
    {
        var requestId = Guid.NewGuid().ToString("N");
        var amount    = (long)Math.Round(order.TotalAmount);
        var orderInfo = $"Thanh toan don hang {order.OrderNumber}";
        var extraData = string.Empty;
        var requestType = "captureWallet";

        var rawHash = $"accessKey={AccessKey}&amount={amount}&extraData={extraData}" +
                      $"&ipnUrl={ipnUrl}&orderId={order.OrderNumber}&orderInfo={orderInfo}" +
                      $"&partnerCode={PartnerCode}&redirectUrl={returnUrl}" +
                      $"&requestId={requestId}&requestType={requestType}";

        var signature = ComputeHmacSha256(rawHash, SecretKey);

        var payload = new
        {
            partnerCode = PartnerCode,
            partnerName = "The Old Pavement Store",
            storeId     = "TheOldPavement",
            requestId,
            amount,
            orderId     = order.OrderNumber,
            orderInfo,
            redirectUrl = returnUrl,
            ipnUrl,
            lang        = "vi",
            extraData,
            requestType,
            signature
        };

        try
        {
            using var client  = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            var json          = JsonSerializer.Serialize(payload);
            var content       = new StringContent(json, Encoding.UTF8, "application/json");
            var httpResponse  = await client.PostAsync(ApiUrl, content, cancellationToken);
            var body          = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
                return Fail($"MoMo API HTTP {(int)httpResponse.StatusCode}");

            var resp = JsonSerializer.Deserialize<MomoCreatePaymentResponse>(body);
            if (resp is null)
                return Fail("Cannot deserialize MoMo response");

            if (!resp.IsSuccess)
                return new MomoPaymentResult { Success = false, ResultCode = resp.ResultCode, ErrorMessage = resp.Message };

            return new MomoPaymentResult
            {
                Success   = true,
                PayUrl    = resp.PayUrl,
                QrCodeUrl = resp.QrCodeUrl,
                ShortLink = resp.ShortLink,
                ResultCode = resp.ResultCode
            };
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }

    // ── VerifyCallbackSignature ───────────────────────────────────────────────

    public bool VerifyCallbackSignature(MomoCallbackParams cb)
    {
        var rawHash = $"accessKey={AccessKey}&amount={cb.Amount}&extraData={cb.ExtraData}" +
                      $"&message={cb.Message}&orderId={cb.OrderId}&orderInfo={cb.OrderInfo}" +
                      $"&partnerCode={cb.PartnerCode}&payType={cb.PayType}&requestId={cb.RequestId}" +
                      $"&responseTime={cb.ResponseTime}&resultCode={cb.ResultCode}&transId={cb.TransId}";

        var computed = ComputeHmacSha256(rawHash, SecretKey);
        return string.Equals(computed, cb.Signature, StringComparison.OrdinalIgnoreCase);
    }

    // ── ProcessCallbackAsync ──────────────────────────────────────────────────

    public async Task<bool> ProcessCallbackAsync(MomoCallbackParams callback, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.FirstOrDefaultAsync(
            o => o.OrderNumber == callback.OrderId);

        if (order == null)
            return false;

        if (order.PaymentStatus == "paid")
            return true;

        if (callback.IsSuccess)
        {
            order.PaymentStatus = "paid";
            order.Status = "confirmed";
            order.TransactionId = callback.TransId;
            order.UpdatedAt = DateTime.Now;
        }
        else
        {
            order.PaymentStatus = "failed";
            order.UpdatedAt = DateTime.Now;
        }

        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        return callback.IsSuccess;
    }

    // ── private ───────────────────────────────────────────────────────────────

    private static string ComputeHmacSha256(string data, string key)
    {
        var keyBytes  = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA256(keyBytes);
        return Convert.ToHexString(hmac.ComputeHash(dataBytes)).ToLowerInvariant();
    }

    private static MomoPaymentResult Fail(string msg) =>
        new() { Success = false, ErrorMessage = msg };
}
