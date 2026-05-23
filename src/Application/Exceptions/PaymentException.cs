namespace Application.Exceptions;

public class PaymentException : Exception
{
    public string PaymentProvider { get; }

    public PaymentException(string message, string paymentProvider = "unknown")
        : base(message)
    {
        PaymentProvider = paymentProvider;
    }
}

