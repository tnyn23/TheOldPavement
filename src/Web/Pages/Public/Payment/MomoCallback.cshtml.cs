using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Context;
using Web.Helpers;

namespace Web.Pages.Public.Payment;

public class MomoCallbackModel : PageModel
{
    private readonly TheOldPavementDbContext _context;
    private readonly IMomoService _momoService;
    private readonly IEmailService _emailService;

    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }

    public MomoCallbackModel(
        TheOldPavementDbContext context, 
        IMomoService momoService, 
        IEmailService emailService)
    {
        _context = context;
        _momoService = momoService;
        _emailService = emailService;
    }

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
        [FromQuery] string signature)
    {
        // 1. Verify signature security
        var isValidSignature = _momoService.ValidateSignature(
            partnerCode, orderId, requestId, amount, orderInfo, orderType, 
            transId, resultCode, message, payType, responseTime, extraData, signature);

        if (!isValidSignature)
        {
            IsSuccess = false;
            Message = "Chữ ký bảo mật không hợp lệ (Signature mismatch). Giao dịch có thể đã bị can thiệp trái phép.";
            return Page();
        }

        // 2. Process based on result code (resultCode == 0 means success)
        if (resultCode == "0")
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderId);

            if (order != null)
            {
                // To avoid multiple updates if user refreshes the callback page
                if (order.PaymentStatus != "paid")
                {
                    order.PaymentStatus = "paid";
                    order.Status = "confirmed";
                    order.UpdatedAt = DateTime.Now;

                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();

                    // Send order confirmation email in the background (fire and forget)
                    _ = Task.Run(() => _emailService.SendOrderConfirmationEmailAsync(order));
                }

                IsSuccess = true;
                OrderNumber = order.OrderNumber;
                Amount = order.TotalAmount;
                Message = "Thanh toán giao dịch thành công. Cảm ơn bạn đã lựa chọn The Old Pavement!";

                // Clear cart only on successful payment
                CartManager.ClearCart(HttpContext.Session);
            }
            else
            {
                IsSuccess = false;
                Message = $"Không tìm thấy đơn hàng '{orderId}' tương ứng trong cơ sở dữ liệu.";
            }
        }
        else
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderNumber == orderId);
            if (order != null && order.Status != "cancelled")
            {
                order.Status = "cancelled";
                order.PaymentStatus = "failed";
                order.UpdatedAt = DateTime.Now;

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }

            IsSuccess = false;
            Message = $"Giao dịch không thành công hoặc đã bị hủy bởi người dùng. (Mã phản hồi MoMo: {resultCode} - {message})";
            OrderNumber = orderId;
        }

        return Page();
    }
}


