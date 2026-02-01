namespace KobiPOS.Models
{
    public class Order
    {
        public int ID { get; set; }
        public int TableID { get; set; }
        public DateTime OrderDate { get; set; }
        public int UserID { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentType { get; set; } = string.Empty; // Nakit, Kredi Kartı, Yemek Kartı
        public string Status { get; set; } = "Bekliyor"; // Bekliyor, Hazırlanıyor, Hazır, Servis Edildi
        public string Notes { get; set; } = string.Empty;
    }
}
