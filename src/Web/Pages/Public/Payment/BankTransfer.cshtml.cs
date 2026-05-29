using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Context;
using Application.Interfaces;

namespace Web.Pages.Public.Payment;

public class BankTransferModel : PageModel
{
    private readonly TheOldPavementDbContext _context;
    private readonly IEmailService _emailService;

    public string OrderNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TransferContent { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = "bank"; // "momo" or "bank"

    // Bank account info
    public string BankName { get; set; } = "MBBank";
    public string AccountNumber { get; set; } = "0965481905";
    public string AccountHolder { get; set; } = "NGUYEN THE HOANG TUNG";
    public string Branch { get; set; } = "Ngân hàng TMCP Quân đội (MB Bank)";

    public BankTransferModel(TheOldPavementDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<IActionResult> OnGetAsync(string orderNumber, decimal amount, string? method)
    {
        if (string.IsNullOrEmpty(orderNumber))
            return RedirectToPage("/Index");

        OrderNumber   = orderNumber;
        Amount        = amount;
        TransferContent = $"TOP {orderNumber}";
        PaymentMethod = method?.ToLower() == "momo" ? "momo" : "bank";

        return Page();
    }

    /// <summary>
    /// Polling endpoint — JS calls this every few seconds to check payment status.
    /// Returns JSON: { status, paid, orderNumber }
    /// </summary>
    public async Task<IActionResult> OnGetCheckStatusAsync(string orderNumber)
    {
        if (string.IsNullOrEmpty(orderNumber))
            return new JsonResult(new { paid = false, status = "unknown" });

        var order = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        if (order == null)
            return new JsonResult(new { paid = false, status = "not_found" });

        var paid = order.PaymentStatus == "paid" || order.Status == "confirmed";

        return new JsonResult(new
        {
            paid,
            status = order.PaymentStatus ?? "pending",
            orderStatus = order.Status ?? "pending",
            orderNumber = order.OrderNumber
        });
    }

    public async Task<IActionResult> OnPostConfirmAsync(string orderNumber)
    {
        if (string.IsNullOrEmpty(orderNumber))
            return RedirectToPage("/Index");

        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        if (order != null && order.PaymentStatus == "pending")
        {
            order.PaymentStatus = "awaiting_confirmation";
            order.UpdatedAt = DateTime.Now;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        // Clear cart
        Web.Helpers.CartManager.ClearCart(HttpContext.Session);

        TempData["OrderedNumber"] = orderNumber;
        return RedirectToPage("/ThankYouCard");
    }

    /// <summary>
    /// Admin confirms payment — called from Dashboard.
    /// Sets PaymentStatus = "paid", Status = "confirmed", sends email.
    /// </summary>
    public async Task<IActionResult> OnPostAdminConfirmPaymentAsync(string orderNumber)
    {
        if (string.IsNullOrEmpty(orderNumber))
            return new JsonResult(new { success = false, message = "Thiếu mã đơn hàng." });

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.ShippingAddress)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        if (order == null)
            return new JsonResult(new { success = false, message = "Không tìm thấy đơn hàng." });

        if (order.PaymentStatus == "paid")
            return new JsonResult(new { success = true, message = "Đơn hàng đã được xác nhận trước đó." });

        order.PaymentStatus = "paid";
        order.Status = "confirmed";
        order.UpdatedAt = DateTime.Now;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        // Send confirmation email
        _ = Task.Run(() => _emailService.SendOrderConfirmationEmailAsync(order));

        return new JsonResult(new { success = true, message = $"Đã xác nhận thanh toán đơn hàng {orderNumber}." });
    }
}
