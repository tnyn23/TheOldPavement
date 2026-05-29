using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces;

/// <summary>
/// Abstraction for MoMo Payment Gateway operations.
/// Implementations must never throw on signature validation failure —
/// return false instead so callers can log and respond appropriately.
/// </summary>
public interface IMomoService
{
    /// <summary>
    /// Creates a payment request with MoMo API and returns the result
    /// containing the payUrl to redirect the user to.
    /// </summary>
    Task<MomoPaymentResult> CreatePaymentAsync(
        Order order,
        string returnUrl,
        string ipnUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the HMAC-SHA256 signature on a MoMo callback.
    /// Returns true only when the computed signature matches exactly.
    /// </summary>
    bool VerifyCallbackSignature(MomoCallbackParams callback);

    /// <summary>
    /// Processes a verified callback: updates order PaymentStatus,
    /// saves TransactionId, and returns whether the payment succeeded.
    /// </summary>
    Task<bool> ProcessCallbackAsync(
        MomoCallbackParams callback,
        CancellationToken cancellationToken = default);
}
