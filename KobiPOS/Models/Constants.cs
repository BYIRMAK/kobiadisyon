namespace KobiPOS.Models
{
    public static class OrderStatus
    {
        public const string Pending = "Bekliyor";
        public const string Preparing = "Hazırlanıyor";
        public const string Ready = "Hazır";
        public const string Served = "Servis Edildi";
    }

    public static class TableStatus
    {
        public const string Empty = "Boş";
        public const string Occupied = "Dolu";
        public const string Reserved = "Rezerve";
    }

    public static class PaymentType
    {
        public const string Cash = "Nakit";
        public const string CreditCard = "Kredi Kartı";
        public const string MealCard = "Yemek Kartı";
    }
}
