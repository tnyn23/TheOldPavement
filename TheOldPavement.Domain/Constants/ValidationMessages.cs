namespace TheOldPavement.Domain.Constants;

public static class ValidationMessages
{
    public const string Required = "{0} is required.";
    public const string MaxLength = "{0} must not exceed {1} characters.";
    public const string MinLength = "{0} must be at least {1} characters.";
    public const string InvalidEmail = "Please enter a valid email address.";
    public const string InvalidPhone = "Please enter a valid phone number.";
    public const string PasswordMismatch = "Password and confirm password do not match.";
    public const string MinPrice = "Price must be greater than 0.";
    public const string MinQuantity = "Quantity must be at least 1.";
    public const string MinRating = "Rating must be between 1 and 5.";
}

