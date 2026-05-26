using Application.DTOs;

namespace Application.Interfaces;

public interface ICheckoutService
{
    Task<CheckoutResultDTO> ProcessCheckoutAsync(CheckoutRequestDTO request);
    Task<PromoValidationResultDTO> ValidatePromoCodeAsync(string code, decimal subtotal);
}
