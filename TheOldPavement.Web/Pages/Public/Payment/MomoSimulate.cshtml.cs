using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheOldPavement.Data.Context;

namespace TheOldPavement.Web.Pages.Public.Payment;

public class MomoSimulateModel : PageModel
{
    private readonly TheOldPavementDbContext _context;
    private readonly IConfiguration _configuration;

    public string OrderId { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string OrderInfo { get; set; } = string.Empty;
    public string StoreName { get; set; } = "The Old Pavement Store";

    public MomoSimulateModel(TheOldPavementDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<IActionResult> OnGetAsync(string orderId, string amount, string orderInfo)
    {
        OrderId = orderId ?? string.Empty;
        Amount = amount ?? "0";
        OrderInfo = orderInfo ?? string.Empty;

        if (string.IsNullOrEmpty(OrderId))
            return RedirectToPage("/Index");

        return Page();
    }

    public IActionResult OnGetSuccess(string orderId, string amount, string orderInfo)
    {
        // Generate a simulated successful MoMo callback
        var accessKey = _configuration["Momo:AccessKey"] ?? "klm05TvNBzhg7h7j";
        var secretKey = _configuration["Momo:SecretKey"] ?? "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";
        var partnerCode = _configuration["Momo:PartnerCode"] ?? "MOMOBKUN20180529";

        var requestId = Guid.NewGuid().ToString();
        var transId = new Random().Next(100000000, 999999999).ToString();
        var responseTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var resultCode = "0";
        var message = "Successful.";
        var payType = "qr";
        var extraData = "";

        // Build signature exactly as ValidateSignature() expects
        var rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&message={message}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&payType={payType}&requestId={requestId}&responseTime={responseTime}&resultCode={resultCode}&transId={transId}";
        var signature = ComputeHmacSha256(rawHash, secretKey);

        // Redirect to MomoCallback with all required query params
        return RedirectToPage("/Public/Payment/MomoCallback", new
        {
            partnerCode,
            orderId,
            requestId,
            amount,
            orderInfo,
            orderType = "momo_wallet",
            transId,
            resultCode,
            message,
            payType,
            responseTime,
            extraData,
            signature
        });
    }

    public IActionResult OnGetCancel(string orderId, string amount, string orderInfo)
    {
        var accessKey = _configuration["Momo:AccessKey"] ?? "klm05TvNBzhg7h7j";
        var secretKey = _configuration["Momo:SecretKey"] ?? "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";
        var partnerCode = _configuration["Momo:PartnerCode"] ?? "MOMOBKUN20180529";

        var requestId = Guid.NewGuid().ToString();
        var transId = "0";
        var responseTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var resultCode = "1006"; // User cancelled
        var message = "Giao dịch bị hủy bởi người dùng.";
        var payType = "qr";
        var extraData = "";

        var rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&message={message}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&payType={payType}&requestId={requestId}&responseTime={responseTime}&resultCode={resultCode}&transId={transId}";
        var signature = ComputeHmacSha256(rawHash, secretKey);

        return RedirectToPage("/Public/Payment/MomoCallback", new
        {
            partnerCode,
            orderId,
            requestId,
            amount,
            orderInfo,
            orderType = "momo_wallet",
            transId,
            resultCode,
            message,
            payType,
            responseTime,
            extraData,
            signature
        });
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
