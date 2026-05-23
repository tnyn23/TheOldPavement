namespace Domain.Constants;

public static class AppSettings
{
    public static class Roles
    {
        public const string Admin = "admin";
        public const string Customer = "customer";
    }

    public static class ProductStatus
    {
        public const string Available = "available";
        public const string SoldOut = "sold_out";
        public const string ComingSoon = "coming_soon";
    }

    public static class OrderStatus
    {
        public const string Pending = "pending";
        public const string Confirmed = "confirmed";
        public const string Shipping = "shipping";
        public const string Delivered = "delivered";
        public const string Cancelled = "cancelled";
    }

    public static class PaymentStatus
    {
        public const string Unpaid = "unpaid";
        public const string Paid = "paid";
    }

    public static class PaymentMethod
    {
        public const string COD = "cod";
        public const string Momo = "momo";
        public const string Stripe = "stripe";
    }

    public static class Pagination
    {
        public const int DefaultPageSize = 12;
        public const int MaxPageSize = 50;
    }
}


