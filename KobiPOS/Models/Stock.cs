namespace KobiPOS.Models
{
    public class Stock
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public string TransactionType { get; set; } = string.Empty; // Giriş, Çıkış
        public DateTime TransactionDate { get; set; }
        public int UserID { get; set; }
    }
}
