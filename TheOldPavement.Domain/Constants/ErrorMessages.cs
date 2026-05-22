namespace TheOldPavement.Domain.Constants;

public static class ErrorMessages
{
    public const string NotFound = "{0} not found.";
    public const string AlreadyExists = "{0} already exists.";
    public const string InvalidCredentials = "Invalid email or password.";
    public const string Unauthorized = "You are not authorized to perform this action.";
    public const string InsufficientStock = "Insufficient stock for product: {0}.";
    public const string InvalidPromoCode = "Invalid or expired promotion code.";
    public const string PaymentFailed = "Payment processing failed. Please try again.";
    public const string InternalServerError = "An unexpected error occurred.";
}

